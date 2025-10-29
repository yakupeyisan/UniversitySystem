using Core.Domain.Events;

namespace Identity.Domain.Events;

public class UserUnlockedEvent : DomainEvent
{
    public UserUnlockedEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}