using Core.Domain;

namespace Identity.Domain.Aggregates;

/// <summary>
/// Ba�ar�s�z giri� deneme kay�tlar� - Brute force korumas� i�in
/// </summary>
public class FailedLoginAttempt : AuditableEntity
{
    private FailedLoginAttempt()
    {
    }

    /// <summary>
    /// Kullan�c� ID (veya email - hen�z do�rulanmam��sa)
    /// </summary>
    public Guid? UserId { get; private set; }

    /// <summary>
    /// Giri� yap�lmaya �al���lan email
    /// </summary>
    public string AttemptedEmail { get; private set; }

    /// <summary>
    /// IP adresi
    /// </summary>
    public string IpAddress { get; private set; }

    /// <summary>
    /// User Agent
    /// </summary>
    public string UserAgent { get; private set; }

    /// <summary>
    /// Hata mesaj�
    /// </summary>
    public string ErrorReason { get; private set; }

    /// <summary>
    /// Deneme zaman�
    /// </summary>
    public DateTime AttemptedAt { get; private set; }

    /// <summary>
    /// Lokasyon bilgisi (IP'den tahmin edilen)
    /// </summary>
    public string Location { get; private set; }

    /// <summary>
    /// Navigation property
    /// </summary>
    public User User { get; private set; }

    public static FailedLoginAttempt Create(
        string attemptedEmail,
        string ipAddress,
        string userAgent,
        string errorReason,
        string location = "",
        Guid? userId = null)
    {
        if (string.IsNullOrWhiteSpace(attemptedEmail))
            throw new ArgumentException("Attempted email cannot be empty", nameof(attemptedEmail));

        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("IP address cannot be empty", nameof(ipAddress));

        return new FailedLoginAttempt
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AttemptedEmail = attemptedEmail.ToLowerInvariant(),
            IpAddress = ipAddress,
            UserAgent = userAgent ?? string.Empty,
            ErrorReason = errorReason ?? string.Empty,
            Location = location ?? string.Empty,
            AttemptedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}