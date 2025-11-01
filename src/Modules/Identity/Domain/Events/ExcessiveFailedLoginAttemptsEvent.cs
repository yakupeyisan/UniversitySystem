using Core.Domain.Events;

namespace Identity.Domain.Events;

/// <summary>
/// Çok fazla baþarýsýz giriþ denemesi event'i
/// </summary>
public class ExcessiveFailedLoginAttemptsEvent : DomainEvent
{
    public ExcessiveFailedLoginAttemptsEvent(Guid userId, string email, int attemptCount)
    {
        UserId = userId;
        Email = email;
        AttemptCount = attemptCount;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public int AttemptCount { get; }
    public DateTime OccurredOn { get; }
}