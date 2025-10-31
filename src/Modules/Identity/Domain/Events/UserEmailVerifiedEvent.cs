using Core.Domain.Events;

namespace Identity.Domain.Events;

/// <summary>
/// Email doðrulama event'i
/// </summary>
public class UserEmailVerifiedEvent : DomainEvent
{
    public UserEmailVerifiedEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; }
}