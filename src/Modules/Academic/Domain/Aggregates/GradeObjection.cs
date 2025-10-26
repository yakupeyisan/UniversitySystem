using Academic.Domain.Enums;
using Academic.Domain.Events;
using Core.Domain;
using Core.Domain.Specifications;

namespace Academic.Domain.Aggregates;

/// <summary>
/// GradeObjection aggregate representing a student's appeal or objection to a grade
/// </summary>
public class GradeObjection : AuditableEntity, ISoftDelete
{
    public Guid GradeId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public DateTime ObjectionDate { get; private set; }
    public DateTime ObjectionDeadline { get; private set; }
    public string Reason { get; private set; } = null!;
    public GradeObjectionStatus Status { get; private set; }
    public int AppealLevel { get; private set; }
    public Guid? ReviewedBy { get; private set; }
    public DateTime? ReviewedDate { get; private set; }
    public string? ReviewNotes { get; private set; }
    public float? NewScore { get; private set; }
    public LetterGrade? NewLetterGrade { get; private set; }

    // Soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public Grade? Grade { get; private set; }

    public void Delete(Guid deletedBy)
    {
        throw new NotImplementedException();
    }

    public void Restore()
    {
        throw new NotImplementedException();
    }

    private GradeObjection() { }

    public static GradeObjection Create(
        Guid gradeId,
        Guid studentId,
        Guid courseId,
        string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Objection reason cannot be empty");

        if (reason.Length > 500)
            throw new ArgumentException("Objection reason cannot exceed 500 characters");

        var objection = new GradeObjection
        {
            Id = Guid.NewGuid(),
            GradeId = gradeId,
            StudentId = studentId,
            CourseId = courseId,
            ObjectionDate = DateTime.UtcNow,
            ObjectionDeadline = DateTime.UtcNow.AddDays(30),
            Reason = reason,
            Status = GradeObjectionStatus.Submitted,
            AppealLevel = 1,
            CreatedAt = DateTime.UtcNow
        };

        objection.AddDomainEvent(new GradeObjectionSubmitted(
            objection.Id,
            gradeId,
            studentId,
            courseId));

        return objection;
    }

    public void SubmitForReview()
    {
        if (Status != GradeObjectionStatus.Submitted)
            throw new InvalidOperationException("Objection is not in submitted status");

        Status = GradeObjectionStatus.UnderReview;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(Guid reviewedBy, string? notes = null, float? newScore = null, LetterGrade? newLetterGrade = null)
    {
        if (Status == GradeObjectionStatus.Approved)
            throw new InvalidOperationException("Objection is already approved");

        Status = GradeObjectionStatus.Approved;
        ReviewedBy = reviewedBy;
        ReviewedDate = DateTime.UtcNow;
        ReviewNotes = notes;
        NewScore = newScore;
        NewLetterGrade = newLetterGrade;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new GradeObjectionApproved(
            Id,
            GradeId,
            StudentId,
            newScore,
            newLetterGrade));
    }

    public void Reject(Guid reviewedBy, string? notes = null)
    {
        if (Status == GradeObjectionStatus.Rejected)
            throw new InvalidOperationException("Objection is already rejected");

        Status = GradeObjectionStatus.Rejected;
        ReviewedBy = reviewedBy;
        ReviewedDate = DateTime.UtcNow;
        ReviewNotes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AppealToNextLevel()
    {
        if (Status != GradeObjectionStatus.Rejected)
            throw new InvalidOperationException("Only rejected objections can be appealed");

        if (AppealLevel >= 3)
            throw new InvalidOperationException("Maximum appeal level reached");

        Status = GradeObjectionStatus.Pending;
        AppealLevel++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Withdraw()
    {
        if (Status == GradeObjectionStatus.Withdrawn)
            throw new InvalidOperationException("Objection is already withdrawn");

        Status = GradeObjectionStatus.Withdrawn;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool CanBeAppealed() => Status == GradeObjectionStatus.Rejected && AppealLevel < 3;

    public bool IsDeadlinePassed() => DateTime.UtcNow > ObjectionDeadline;

    public bool IsPending() => Status == GradeObjectionStatus.Pending || Status == GradeObjectionStatus.UnderReview;
}