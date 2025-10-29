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

    /// <summary>
    ///     �ifresi s�f�rlanan kullan�c� ID'si
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    ///     Kullan�c� email adresi
    /// </summary>
    public string Email { get; }

    /// <summary>
    ///     Event olu�turulma zaman� (UTC)
    /// </summary>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}