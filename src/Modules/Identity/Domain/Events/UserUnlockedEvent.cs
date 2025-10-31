using Core.Domain.Events;

namespace Identity.Domain.Events;

/// <summary>
/// Kullan�c� hesab� a��ld� event'i
/// </summary>
public class UserUnlockedEvent : DomainEvent
{
    public UserUnlockedEvent(Guid userId, string reason)
    {
        UserId = userId;
        Reason = reason;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public string Reason { get; }
    public DateTime OccurredOn { get; }
}