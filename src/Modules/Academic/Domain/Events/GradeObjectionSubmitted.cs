using Core.Domain.Events;

namespace Academic.Domain.Events;

/// <summary>
/// Event raised when a grade objection is submitted
/// </summary>
public class GradeObjectionSubmitted : DomainEvent
{
    public Guid ObjectionId { get; }
    public Guid GradeId { get; }
    public Guid StudentId { get; }
    public Guid CourseId { get; }
    public Guid AggregateId => ObjectionId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public GradeObjectionSubmitted(Guid objectionId, Guid gradeId, Guid studentId, Guid courseId)
    {
        ObjectionId = objectionId;
        GradeId = gradeId;
        StudentId = studentId;
        CourseId = courseId;
    }
}