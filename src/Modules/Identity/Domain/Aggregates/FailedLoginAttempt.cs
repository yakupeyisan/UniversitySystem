using Core.Domain;
namespace Identity.Domain.Aggregates;
public class FailedLoginAttempt : AuditableEntity
{
    private FailedLoginAttempt()
    {
    }
    public Guid? UserId { get; private set; }
    public string AttemptedEmail { get; private set; }
    public string IpAddress { get; private set; }
    public string UserAgent { get; private set; }
    public string ErrorReason { get; private set; }
    public DateTime AttemptedAt { get; private set; }
    public string Location { get; private set; }
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