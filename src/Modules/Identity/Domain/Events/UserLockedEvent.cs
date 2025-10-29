using Core.Domain.Events;

namespace Identity.Domain.Events;

public class UserLockedEvent : DomainEvent
{
    public UserLockedEvent(Guid userId, string email, DateTime lockedUntil, string? reason = "Account locked")
    {
        UserId = userId;
        Email = email;
        LockedUntil = lockedUntil;
        Reason = reason;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public DateTime LockedUntil { get; }
    public string Reason { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}