using Core.Domain.Events;

namespace Academic.Domain.Events;

/// <summary>
/// Event raised when a student registers for a course
/// </summary>
public class CourseRegistrationCreated : DomainEvent
{
    public Guid RegistrationId { get; }
    public Guid StudentId { get; }
    public Guid CourseId { get; }
    public string Semester { get; }
    public bool IsRetake { get; }
    public Guid AggregateId => RegistrationId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public CourseRegistrationCreated(Guid registrationId, Guid studentId, Guid courseId, string semester, bool isRetake)
    {
        RegistrationId = registrationId;
        StudentId = studentId;
        CourseId = courseId;
        Semester = semester;
        IsRetake = isRetake;
    }
}