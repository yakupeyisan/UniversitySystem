using Core.Domain.Events;
namespace Academic.Domain.Events;
public class PrerequisiteWaiverApproved : DomainEvent
{
    public Guid WaiverId { get; }
    public Guid StudentId { get; }
    public Guid PrerequisiteId { get; }
    public Guid CourseId { get; }
    public Guid AggregateId => WaiverId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public PrerequisiteWaiverApproved(Guid waiverId, Guid studentId, Guid prerequisiteId, Guid courseId)
    {
        WaiverId = waiverId;
        StudentId = studentId;
        PrerequisiteId = prerequisiteId;
        CourseId = courseId;
    }
}