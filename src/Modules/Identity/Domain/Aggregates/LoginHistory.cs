using Core.Domain;
using Identity.Domain.Enums;
namespace Identity.Domain.Aggregates;
public class LoginHistory : AuditableEntity
{
    private LoginHistory()
    {
    }
    public Guid UserId { get; private set; }
    public DateTime LoginAt { get; private set; }
    public DateTime? LogoutAt { get; private set; }
    public string IpAddress { get; private set; }
    public string UserAgent { get; private set; }
    public string OperatingSystem { get; private set; }
    public string BrowserName { get; private set; }
    public string DeviceType { get; private set; }
    public string Location { get; private set; }
    public LoginResultType Result { get; private set; }
    public string ErrorMessage { get; private set; }
    public int? SessionDurationMinutes { get; private set; }
    public bool IsTwoFactorUsed { get; private set; }
    public bool AccessTokenCreated { get; private set; }
    public bool RefreshTokenCreated { get; private set; }
    public User User { get; private set; }
    public static LoginHistory CreateSuccess(
        Guid userId,
        DateTime loginAt,
        string ipAddress,
        string userAgent,
        string operatingSystem,
        string browserName,
        string deviceType,
        string location = "",
        bool isTwoFactorUsed = false)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("IP address cannot be empty", nameof(ipAddress));
        return new LoginHistory
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            LoginAt = loginAt,
            IpAddress = ipAddress,
            UserAgent = userAgent ?? string.Empty,
            OperatingSystem = operatingSystem ?? string.Empty,
            BrowserName = browserName ?? string.Empty,
            DeviceType = deviceType ?? string.Empty,
            Location = location ?? string.Empty,
            Result = LoginResultType.Success,
            IsTwoFactorUsed = isTwoFactorUsed,
            AccessTokenCreated = true,
            RefreshTokenCreated = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
    public static LoginHistory CreateFailed(
        Guid userId,
        DateTime loginAt,
        string ipAddress,
        string userAgent,
        string errorMessage,
        string operatingSystem = "",
        string browserName = "",
        string deviceType = "",
        string location = "")
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        return new LoginHistory
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            LoginAt = loginAt,
            IpAddress = ipAddress,
            UserAgent = userAgent ?? string.Empty,
            OperatingSystem = operatingSystem ?? string.Empty,
            BrowserName = browserName ?? string.Empty,
            DeviceType = deviceType ?? string.Empty,
            Location = location ?? string.Empty,
            Result = LoginResultType.Failed,
            ErrorMessage = errorMessage ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
    public static LoginHistory CreateLocked(
        Guid userId,
        DateTime loginAt,
        string ipAddress,
        string userAgent,
        string operatingSystem = "",
        string browserName = "",
        string deviceType = "",
        string location = "")
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        return new LoginHistory
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            LoginAt = loginAt,
            IpAddress = ipAddress,
            UserAgent = userAgent ?? string.Empty,
            OperatingSystem = operatingSystem ?? string.Empty,
            BrowserName = browserName ?? string.Empty,
            DeviceType = deviceType ?? string.Empty,
            Location = location ?? string.Empty,
            Result = LoginResultType.Locked,
            ErrorMessage = "Account is locked",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
    public void RecordLogout()
    {
        LogoutAt = DateTime.UtcNow;
        if (LoginAt < LogoutAt)
        {
            SessionDurationMinutes = (int)(LogoutAt.Value - LoginAt).TotalMinutes;
        }
        UpdatedAt = DateTime.UtcNow;
    }
    public bool IsSessionActive(int sessionTimeoutMinutes = 30)
    {
        if (LogoutAt.HasValue)
            return false;
        var sessionExpiry = LoginAt.AddMinutes(sessionTimeoutMinutes);
        return DateTime.UtcNow < sessionExpiry;
    }
}