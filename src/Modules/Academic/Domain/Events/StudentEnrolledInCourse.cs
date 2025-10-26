using Core.Domain.Events;

namespace Academic.Domain.Events;

/// <summary>
/// Event raised when a student enrolls in a course
/// </summary>
public class StudentEnrolledInCourse : DomainEvent
{
    public Guid RegistrationId { get; }
    public Guid StudentId { get; }
    public Guid CourseId { get; }
    public Guid AggregateId => RegistrationId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public StudentEnrolledInCourse(Guid registrationId, Guid studentId, Guid courseId)
    {
        RegistrationId = registrationId;
        StudentId = studentId;
        CourseId = courseId;
    }
}