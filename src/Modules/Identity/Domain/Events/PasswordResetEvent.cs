using Core.Domain.Events;
namespace Identity.Domain.Events;
public class PasswordResetEvent : DomainEvent
{
    public PasswordResetEvent(Guid userId, string email)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        UserId = userId;
        Email = email;
    }
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}