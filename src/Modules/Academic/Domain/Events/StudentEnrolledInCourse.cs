using Core.Domain.Events;

namespace Academic.Domain.Events;

public class StudentEnrolledInCourse : DomainEvent
{
    public StudentEnrolledInCourse(Guid registrationId, Guid studentId, Guid courseId)
    {
        RegistrationId = registrationId;
        StudentId = studentId;
        CourseId = courseId;
    }

    public Guid RegistrationId { get; }
    public Guid StudentId { get; }
    public Guid CourseId { get; }
    public Guid AggregateId => RegistrationId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}