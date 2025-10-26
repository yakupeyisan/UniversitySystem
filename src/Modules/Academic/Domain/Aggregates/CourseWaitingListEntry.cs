using Academic.Domain.Enums;
using Academic.Domain.Events;
using Core.Domain;
using Core.Domain.Specifications;

namespace Academic.Domain.Aggregates;

/// <summary>
/// CourseWaitingListEntry aggregate representing a student's position on a course waiting list
/// </summary>
public class CourseWaitingListEntry : AuditableEntity, ISoftDelete
{
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public int QueuePosition { get; private set; }
    public WaitingListStatus Status { get; private set; }
    public DateTime RequestedDate { get; private set; }
    public DateTime? AdmittedDate { get; private set; }
    public DateTime? CancelledDate { get; private set; }
    public string? CancelReason { get; private set; }

    // Soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public Course? Course { get; private set; }
    public void Delete(Guid deletedBy)
    {
        throw new NotImplementedException();
    }

    public void Restore()
    {
        throw new NotImplementedException();
    }

    private CourseWaitingListEntry() { }

    public static CourseWaitingListEntry Create(
        Guid studentId,
        Guid courseId,
        int queuePosition)
    {
        if (queuePosition <= 0)
            throw new ArgumentException("Queue position must be greater than 0");

        var entry = new CourseWaitingListEntry
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            CourseId = courseId,
            QueuePosition = queuePosition,
            Status = WaitingListStatus.Waiting,
            RequestedDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        return entry;
    }

    public void PromoteToEnrolled()
    {
        if (Status != WaitingListStatus.Waiting)
            throw new InvalidOperationException("Only waiting students can be promoted");

        Status = WaitingListStatus.Admitted;
        AdmittedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new StudentPromotedFromWaitingList(Id, StudentId, CourseId));
    }

    public void RemoveFromWaitingList(string reason)
    {
        if (Status == WaitingListStatus.Cancelled)
            throw new InvalidOperationException("Already removed from waiting list");

        Status = WaitingListStatus.Cancelled;
        CancelledDate = DateTime.UtcNow;
        CancelReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateQueuePosition(int newPosition)
    {
        if (newPosition <= 0)
            throw new ArgumentException("Queue position must be greater than 0");

        if (Status != WaitingListStatus.Waiting)
            throw new InvalidOperationException("Cannot update position for non-waiting students");

        QueuePosition = newPosition;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool CanBePromoted() => Status == WaitingListStatus.Waiting;

    public int GetWaitingPosition() => QueuePosition;

    public bool IsAdmitted() => Status == WaitingListStatus.Admitted;

    public bool IsCancelled() => Status == WaitingListStatus.Cancelled;
}