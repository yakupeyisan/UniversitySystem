using Core.Domain.Events;
namespace Academic.Domain.Events;
public class GradeObjectionSubmitted : DomainEvent
{
    public GradeObjectionSubmitted(Guid objectionId, Guid gradeId, Guid studentId, Guid courseId)
    {
        ObjectionId = objectionId;
        GradeId = gradeId;
        StudentId = studentId;
        CourseId = courseId;
    }
    public Guid ObjectionId { get; }
    public Guid GradeId { get; }
    public Guid StudentId { get; }
    public Guid CourseId { get; }
    public Guid AggregateId => ObjectionId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}