using Core.Domain.Events;

namespace Academic.Domain.Events;

/// <summary>
/// Event raised when a student is promoted from the waiting list
/// </summary>
public class StudentPromotedFromWaitingList : DomainEvent
{
    public Guid WaitingListEntryId { get; }
    public Guid StudentId { get; }
    public Guid CourseId { get; }
    public Guid AggregateId => WaitingListEntryId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public StudentPromotedFromWaitingList(Guid waitingListEntryId, Guid studentId, Guid courseId)
    {
        WaitingListEntryId = waitingListEntryId;
        StudentId = studentId;
        CourseId = courseId;
    }
}