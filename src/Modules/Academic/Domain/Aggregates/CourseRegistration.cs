using Academic.Domain.Enums;
using Academic.Domain.Events;
using Core.Domain;
using Core.Domain.Specifications;
namespace Academic.Domain.Aggregates;
public class CourseRegistration : AuditableEntity, ISoftDelete
{
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public string Semester { get; private set; } = null!;
    public DateTime RegistrationDate { get; private set; }
    public RegistrationStatus Status { get; private set; }
    public DateTime? DropDate { get; private set; }
    public string? DropReason { get; private set; }
    public bool IsRetake { get; private set; }
    public Guid? PreviousGradeId { get; private set; }
    public Guid? GradeId { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }
    public Course? Course { get; private set; }
    public void Delete(Guid deletedBy)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Course registration is already deleted");
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deletedBy;
    }
    public void Restore()
    {
        if (!IsDeleted)
            throw new InvalidOperationException("Course registration is not deleted");
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        UpdatedAt = DateTime.UtcNow;
    }
    private CourseRegistration() { }
    public static CourseRegistration Create(
        Guid studentId,
        Guid courseId,
        string semester,
        bool isRetake = false,
        Guid? previousGradeId = null)
    {
        if (studentId == Guid.Empty)
            throw new ArgumentException("Student ID cannot be empty");
        if (courseId == Guid.Empty)
            throw new ArgumentException("Course ID cannot be empty");
        if (string.IsNullOrWhiteSpace(semester))
            throw new ArgumentException("Semester cannot be empty");
        var registration = new CourseRegistration
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            CourseId = courseId,
            Semester = semester,
            RegistrationDate = DateTime.UtcNow,
            Status = RegistrationStatus.Registered,
            IsRetake = isRetake,
            PreviousGradeId = previousGradeId,
            CreatedAt = DateTime.UtcNow
        };
        registration.AddDomainEvent(new StudentEnrolledInCourse(
            registration.Id,
            studentId,
            courseId));
        return registration;
    }
    public void Drop(string reason)
    {
        if (Status == RegistrationStatus.Dropped)
            throw new InvalidOperationException("Course is already dropped");
        if (Status == RegistrationStatus.Completed)
            throw new InvalidOperationException("Cannot drop a completed course");
        Status = RegistrationStatus.Dropped;
        DropDate = DateTime.UtcNow;
        DropReason = reason;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new StudentDroppedCourse(
            Id,
            StudentId,
            CourseId,
            reason));
    }
    public void Complete()
    {
        if (Status == RegistrationStatus.Completed)
            throw new InvalidOperationException("Course is already completed");
        if (Status == RegistrationStatus.Dropped)
            throw new InvalidOperationException("Cannot complete a dropped course");
        Status = RegistrationStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Fail()
    {
        if (Status == RegistrationStatus.Failed)
            throw new InvalidOperationException("Course is already marked as failed");
        Status = RegistrationStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
    }
    public void AssignGrade(Guid gradeId)
    {
        if (GradeId.HasValue)
            throw new InvalidOperationException("Grade is already assigned");
        GradeId = gradeId;
        UpdatedAt = DateTime.UtcNow;
    }
    public bool CanDrop() => Status == RegistrationStatus.Registered || Status == RegistrationStatus.WaitingList;
    public bool IsActive() => Status == RegistrationStatus.Registered && !IsDeleted;
}