using Core.Domain.Events;
namespace Academic.Domain.Events;
public class CourseRegistrationCreated : DomainEvent
{
    public CourseRegistrationCreated(Guid registrationId, Guid studentId, Guid courseId, string semester, bool isRetake)
    {
        RegistrationId = registrationId;
        StudentId = studentId;
        CourseId = courseId;
        Semester = semester;
        IsRetake = isRetake;
    }
    public Guid RegistrationId { get; }
    public Guid StudentId { get; }
    public Guid CourseId { get; }
    public string Semester { get; }
    public bool IsRetake { get; }
    public Guid AggregateId => RegistrationId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}