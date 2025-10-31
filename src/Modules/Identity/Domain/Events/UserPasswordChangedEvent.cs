using Core.Domain.Events;

namespace Identity.Domain.Events;

/// <summary>
/// Parola deðiþtirildi event'i
/// </summary>
public class UserPasswordChangedEvent : DomainEvent
{
    public UserPasswordChangedEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; }
}