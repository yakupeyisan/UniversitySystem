using Core.Domain.Events;

namespace Identity.Domain.Events;

/// <summary>
/// Parola sýfýrlandý event'i
/// </summary>
public class UserPasswordResetEvent : DomainEvent
{
    public UserPasswordResetEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; }
}