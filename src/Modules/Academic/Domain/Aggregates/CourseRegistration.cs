using Academic.Domain.Enums;
using Academic.Domain.Events;
using Core.Domain;
using Core.Domain.Specifications;

namespace Academic.Domain.Aggregates;

/// <summary>
/// CourseRegistration aggregate representing a student's enrollment in a course
/// </summary>
public class CourseRegistration : AuditableEntity, ISoftDelete
{
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public string Semester { get; private set; } = null!; // Format: "2024-Fall"
    public DateTime RegistrationDate { get; private set; }
    public RegistrationStatus Status { get; private set; }
    public DateTime? DropDate { get; private set; }
    public string? DropReason { get; private set; }
    public bool IsRetake { get; private set; }
    public Guid? PreviousGradeId { get; private set; }
    public Guid? GradeId { get; private set; }

    // Soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public Course? Course { get; private set; }
    public void Delete(Guid deletedBy)
    {
        throw new NotImplementedException();
    }

    public void Restore()
    {
        throw new NotImplementedException();
    }

    private CourseRegistration() { }

    public static CourseRegistration Create(
        Guid studentId,
        Guid courseId,
        string semester,
        bool isRetake = false,
        Guid? previousGradeId = null)
    {
        if (string.IsNullOrWhiteSpace(semester))
            throw new ArgumentException("Semester cannot be empty");

        if (isRetake && previousGradeId == null)
            throw new ArgumentException("Previous grade ID must be provided for retakes");

        var registration = new CourseRegistration
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            CourseId = courseId,
            Semester = semester,
            RegistrationDate = DateTime.UtcNow,
            Status = RegistrationStatus.Active,
            IsRetake = isRetake,
            PreviousGradeId = previousGradeId,
            CreatedAt = DateTime.UtcNow
        };

        registration.AddDomainEvent(new CourseRegistrationCreated(
            registration.Id,
            studentId,
            courseId,
            semester,
            isRetake));

        return registration;
    }

    public void DropCourse(string reason)
    {
        if (Status == RegistrationStatus.Dropped)
            throw new InvalidOperationException("Course is already dropped");

        if (Status == RegistrationStatus.Completed)
            throw new InvalidOperationException("Cannot drop a completed course");

        Status = RegistrationStatus.Dropped;
        DropDate = DateTime.UtcNow;
        DropReason = reason;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new StudentDroppedCourse(Id, StudentId, CourseId, reason));
    }

    public void WithdrawCourse(string reason)
    {
        if (Status == RegistrationStatus.Withdrawn)
            throw new InvalidOperationException("Course is already withdrawn");

        if (Status == RegistrationStatus.Completed)
            throw new InvalidOperationException("Cannot withdraw from a completed course");

        Status = RegistrationStatus.Withdrawn;
        DropDate = DateTime.UtcNow;
        DropReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignGrade(Guid gradeId)
    {
        if (Status != RegistrationStatus.Active)
            throw new InvalidOperationException("Cannot assign grade to non-active registration");

        GradeId = gradeId;
        Status = RegistrationStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool CanDropCourse()
    {
        return Status == RegistrationStatus.Active;
    }

    public bool IsCompleted() => Status == RegistrationStatus.Completed;

    public bool IsActive() => Status == RegistrationStatus.Active;
}