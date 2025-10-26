using Core.Domain.Events;

namespace Academic.Domain.Events;

/// <summary>
/// Event raised when a student drops a course
/// </summary>
public class StudentDroppedCourse : DomainEvent
{
    public Guid RegistrationId { get; }
    public Guid StudentId { get; }
    public Guid CourseId { get; }
    public string Reason { get; }
    public Guid AggregateId => RegistrationId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public StudentDroppedCourse(Guid registrationId, Guid studentId, Guid courseId, string reason)
    {
        RegistrationId = registrationId;
        StudentId = studentId;
        CourseId = courseId;
        Reason = reason;
    }
}