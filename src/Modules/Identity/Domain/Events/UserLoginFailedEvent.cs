using Core.Domain.Events;

namespace Identity.Domain.Events;

public class UserLoginFailedEvent : DomainEvent
{
    public UserLoginFailedEvent(Guid userId, string email, int attemptCount)
    {
        UserId = userId;
        Email = email;
        AttemptCount = attemptCount;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public int AttemptCount { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}