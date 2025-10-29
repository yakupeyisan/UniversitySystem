using Core.Domain.Events;

namespace Identity.Domain.Events;

public class UserDeletedEvent : DomainEvent
{
    public UserDeletedEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}