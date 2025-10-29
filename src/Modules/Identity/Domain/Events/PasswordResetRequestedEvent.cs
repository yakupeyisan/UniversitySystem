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
    ///     Þifre sýfýrlama isteði yapan kullanýcý ID'si
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    ///     Kullanýcý email adresi
    /// </summary>
    public string Email { get; }

    /// <summary>
    ///     Þifre sýfýrlama kodu (token)
    /// </summary>
    public string ResetCode { get; }

    /// <summary>
    ///     Event oluþturulma zamaný (UTC)
    /// </summary>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}