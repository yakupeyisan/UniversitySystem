using Core.Domain.Events;
using Identity.Domain.Enums;
namespace Identity.Domain.Events;
public class UserLockedEvent : DomainEvent
{
    public UserLockedEvent(Guid userId, string reason)
    {
        UserId = userId;
        Reason = reason;
        OccurredOn = DateTime.UtcNow;
    }
    public Guid UserId { get; }
    public string Reason { get; }
    public DateTime OccurredOn { get; }
}