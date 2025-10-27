using Core.Domain.Events;

namespace Identity.Domain.Events;

public class UserEmailVerifiedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserEmailVerifiedEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}