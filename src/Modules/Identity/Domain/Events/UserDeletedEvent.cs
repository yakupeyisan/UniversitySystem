using Core.Domain.Events;

namespace Identity.Domain.Events;

/// <summary>
/// Kullan�c� silindi event'i
/// </summary>
public class UserDeletedEvent : DomainEvent
{
    public UserDeletedEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; }
}