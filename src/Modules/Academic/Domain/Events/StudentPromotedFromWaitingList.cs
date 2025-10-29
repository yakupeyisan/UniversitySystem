using Core.Domain.Events;

namespace Academic.Domain.Events;

public class StudentPromotedFromWaitingList : DomainEvent
{
    public StudentPromotedFromWaitingList(Guid waitingListEntryId, Guid studentId, Guid courseId)
    {
        WaitingListEntryId = waitingListEntryId;
        StudentId = studentId;
        CourseId = courseId;
    }

    public Guid WaitingListEntryId { get; }
    public Guid StudentId { get; }
    public Guid CourseId { get; }
    public Guid AggregateId => WaitingListEntryId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}