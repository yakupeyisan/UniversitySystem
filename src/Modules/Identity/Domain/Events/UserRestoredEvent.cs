using Core.Domain.Events;

namespace Identity.Domain.Events;

/// <summary>
/// Silinen kullanýcý geri alýndý event'i
/// </summary>
public class UserRestoredEvent : DomainEvent
{
    public UserRestoredEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; }
}