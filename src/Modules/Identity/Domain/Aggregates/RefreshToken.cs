using Core.Domain;
namespace Identity.Domain.Aggregates;
public class RefreshToken : AuditableEntity
{
    private RefreshToken()
    {
    }
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string RevokeReason { get; private set; }
    public string IpAddress { get; private set; }
    public string UserAgent { get; private set; }
    public User User { get; private set; }
    public bool IsExpired => DateTime.UtcNow > ExpiryDate;
    public bool IsValid => !IsRevoked && !IsExpired;
    public static RefreshToken Create(
        Guid userId,
        string token,
        DateTime expiryDate,
        string ipAddress,
        string userAgent)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be empty", nameof(token));
        if (expiryDate <= DateTime.UtcNow)
            throw new ArgumentException("Expiry date must be in the future", nameof(expiryDate));
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiryDate = expiryDate,
            IsRevoked = false,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
    public void Revoke(string reason = "")
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        RevokeReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }
}