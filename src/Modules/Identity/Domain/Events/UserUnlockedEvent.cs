using Core.Domain.Events;
namespace Identity.Domain.Events;
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