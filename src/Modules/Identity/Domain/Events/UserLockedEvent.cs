using Core.Domain.Events;
using Identity.Domain.Enums;

namespace Identity.Domain.Events;

public class UserLockedEvent : DomainEvent
{
    public UserLockedEvent(Guid userId, AccountLockoutReason reason, string details = "")
    {
        UserId = userId;
        Reason = reason;
        Details = details;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public AccountLockoutReason Reason { get; }
    public string Details { get; }
    public DateTime OccurredOn { get; }
}