using Academic.Domain.Enums;
using Academic.Domain.Events;
using Core.Domain;
using Core.Domain.Specifications;
namespace Academic.Domain.Aggregates;
public class GradeObjection : AuditableEntity, ISoftDelete
{
    private GradeObjection()
    {
    }
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
    public Grade? Grade { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }
    public void Delete(Guid deletedBy)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Grade objection is already deleted");
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deletedBy;
    }
    public void Restore()
    {
        if (!IsDeleted)
            throw new InvalidOperationException("Grade objection is not deleted");
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        UpdatedAt = DateTime.UtcNow;
    }
    public static GradeObjection Create(
        Guid gradeId,
        Guid studentId,
        Guid courseId,
        string reason)
    {
        if (gradeId == Guid.Empty)
            throw new ArgumentException("Grade ID cannot be empty");
        if (studentId == Guid.Empty)
            throw new ArgumentException("Student ID cannot be empty");
        if (courseId == Guid.Empty)
            throw new ArgumentException("Course ID cannot be empty");
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
    public void Approve(
        Guid reviewedBy,
        string? notes = null,
        float? newScore = null,
        LetterGrade? newLetterGrade = null)
    {
        if (Status != GradeObjectionStatus.UnderReview && Status != GradeObjectionStatus.Escalated)
            throw new InvalidOperationException("Objection is not under review");
        if (reviewedBy == Guid.Empty)
            throw new ArgumentException("Reviewer ID cannot be empty");
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
        if (Status != GradeObjectionStatus.UnderReview && Status != GradeObjectionStatus.Escalated)
            throw new InvalidOperationException("Objection is not under review");
        if (reviewedBy == Guid.Empty)
            throw new ArgumentException("Reviewer ID cannot be empty");
        Status = GradeObjectionStatus.Rejected;
        ReviewedBy = reviewedBy;
        ReviewedDate = DateTime.UtcNow;
        ReviewNotes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Escalate()
    {
        if (Status != GradeObjectionStatus.UnderReview)
            throw new InvalidOperationException("Only objections under review can be escalated");
        if (AppealLevel >= 3)
            throw new InvalidOperationException("Maximum appeal level reached");
        Status = GradeObjectionStatus.Escalated;
        AppealLevel++;
        UpdatedAt = DateTime.UtcNow;
    }
    public bool CanBeSubmitted()
    {
        return DateTime.UtcNow <= ObjectionDeadline && Status == GradeObjectionStatus.Submitted;
    }
    public bool IsPending()
    {
        return Status == GradeObjectionStatus.Submitted || Status == GradeObjectionStatus.Pending;
    }
    public bool IsResolved()
    {
        return Status == GradeObjectionStatus.Approved || Status == GradeObjectionStatus.Rejected;
    }
}