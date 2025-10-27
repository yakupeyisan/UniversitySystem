using Academic.Domain.Enums;
using Academic.Domain.Events;
using Core.Domain;
using Core.Domain.Specifications;
namespace Academic.Domain.Aggregates;
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
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }
    public Course? Course { get; private set; }
    public void Delete(Guid deletedBy)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Waiting list entry is already deleted");
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deletedBy;
    }
    public void Restore()
    {
        if (!IsDeleted)
            throw new InvalidOperationException("Waiting list entry is not deleted");
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        UpdatedAt = DateTime.UtcNow;
    }
    private CourseWaitingListEntry() { }
    public static CourseWaitingListEntry Create(
        Guid studentId,
        Guid courseId,
        int queuePosition)
    {
        if (studentId == Guid.Empty)
            throw new ArgumentException("Student ID cannot be empty");
        if (courseId == Guid.Empty)
            throw new ArgumentException("Course ID cannot be empty");
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
    public void Admit()
    {
        if (Status != WaitingListStatus.Waiting)
            throw new InvalidOperationException("Only waiting entries can be admitted");
        Status = WaitingListStatus.Admitted;
        AdmittedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Cancel(string reason)
    {
        if (Status == WaitingListStatus.Cancelled)
            throw new InvalidOperationException("Entry is already cancelled");
        if (Status == WaitingListStatus.Admitted)
            throw new InvalidOperationException("Cannot cancel an admitted entry");
        Status = WaitingListStatus.Cancelled;
        CancelledDate = DateTime.UtcNow;
        CancelReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Expire()
    {
        if (Status == WaitingListStatus.Expired)
            throw new InvalidOperationException("Entry is already expired");
        if (Status == WaitingListStatus.Admitted)
            throw new InvalidOperationException("Cannot expire an admitted entry");
        Status = WaitingListStatus.Expired;
        UpdatedAt = DateTime.UtcNow;
    }
    public void UpdateQueuePosition(int newPosition)
    {
        if (newPosition <= 0)
            throw new ArgumentException("Queue position must be greater than 0");
        QueuePosition = newPosition;
        UpdatedAt = DateTime.UtcNow;
    }
    public bool IsWaiting() => Status == WaitingListStatus.Waiting;
    public bool CanBeAdmitted() => Status == WaitingListStatus.Waiting;
    public bool IsActive() => Status == WaitingListStatus.Waiting && !IsDeleted;
}