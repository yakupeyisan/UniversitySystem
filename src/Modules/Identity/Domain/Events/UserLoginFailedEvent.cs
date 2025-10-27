using Core.Domain.Events;

namespace Identity.Domain.Events;

public class UserLoginFailedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public int AttemptCount { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserLoginFailedEvent(Guid userId, string email, int attemptCount)
    {
        UserId = userId;
        Email = email;
        AttemptCount = attemptCount;
    }
}