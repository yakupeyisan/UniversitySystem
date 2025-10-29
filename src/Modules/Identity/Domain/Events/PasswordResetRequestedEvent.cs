using Core.Domain.Events;

namespace Identity.Domain.Events;

public class PasswordResetRequestedEvent : DomainEvent
{
    public PasswordResetRequestedEvent(Guid userId, string email, string resetCode)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (string.IsNullOrWhiteSpace(resetCode))
            throw new ArgumentException("Reset code cannot be empty", nameof(resetCode));

        UserId = userId;
        Email = email;
        ResetCode = resetCode;
    }

    /// <summary>
    ///     �ifre s�f�rlama iste�i yapan kullan�c� ID'si
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    ///     Kullan�c� email adresi
    /// </summary>
    public string Email { get; }

    /// <summary>
    ///     �ifre s�f�rlama kodu (token)
    /// </summary>
    public string ResetCode { get; }

    /// <summary>
    ///     Event olu�turulma zaman� (UTC)
    /// </summary>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}