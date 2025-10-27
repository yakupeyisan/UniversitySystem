using Core.Domain.Events;

namespace Identity.Domain.Events;

public class UserLockedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime LockedUntil { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserLockedEvent(Guid userId, string email, DateTime lockedUntil)
    {
        UserId = userId;
        Email = email;
        LockedUntil = lockedUntil;
    }
}