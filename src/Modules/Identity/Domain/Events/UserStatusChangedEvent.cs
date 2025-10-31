using Core.Domain.Events;

namespace Identity.Domain.Events;

/// <summary>
/// Durumu deðiþtirildi event'i
/// </summary>
public class UserStatusChangedEvent : DomainEvent
{
    public UserStatusChangedEvent(Guid userId, string newStatus)
    {
        UserId = userId;
        NewStatus = newStatus;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public string NewStatus { get; }
    public DateTime OccurredOn { get; }
}