using Core.Domain;
using Identity.Domain.Enums;

namespace Identity.Domain.Aggregates;

/// <summary>
/// Kullan�c� giri� ge�mi�i - Audit log ve g�venlik analizi i�in
/// </summary>
public class LoginHistory : AuditableEntity
{
    private LoginHistory()
    {
    }

    /// <summary>
    /// Kullan�c� ID
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Giri� tarihi ve saati
    /// </summary>
    public DateTime LoginAt { get; private set; }

    /// <summary>
    /// ��k�� tarihi ve saati (logout)
    /// </summary>
    public DateTime? LogoutAt { get; private set; }

    /// <summary>
    /// Istemci IP adresi
    /// </summary>
    public string IpAddress { get; private set; }

    /// <summary>
    /// User Agent (taray�c�/cihaz bilgisi)
    /// </summary>
    public string UserAgent { get; private set; }

    /// <summary>
    /// ��letim Sistemi
    /// </summary>
    public string OperatingSystem { get; private set; }

    /// <summary>
    /// Taray�c� ad�
    /// </summary>
    public string BrowserName { get; private set; }

    /// <summary>
    /// Cihaz tipi (Desktop/Mobile/Tablet)
    /// </summary>
    public string DeviceType { get; private set; }

    /// <summary>
    /// Co�rafi konum (IP'den tahmin edilen)
    /// </summary>
    public string Location { get; private set; }

    /// <summary>
    /// Giri� sonucu (Success/Failed/Locked)
    /// </summary>
    public LoginResultType Result { get; private set; }

    /// <summary>
    /// Hata mesaj� (varsa)
    /// </summary>
    public string ErrorMessage { get; private set; }

    /// <summary>
    /// Oturum s�resi (dakika cinsinden)
    /// </summary>
    public int? SessionDurationMinutes { get; private set; }

    /// <summary>
    /// 2FA kullan�ld� m�
    /// </summary>
    public bool IsTwoFactorUsed { get; private set; }

    /// <summary>
    /// Access token olu�turuldu m�
    /// </summary>
    public bool AccessTokenCreated { get; private set; }

    /// <summary>
    /// Refresh token olu�turuldu m�
    /// </summary>
    public bool RefreshTokenCreated { get; private set; }

    /// <summary>
    /// Navigation property
    /// </summary>
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

    /// <summary>
    /// ��k�� i�lemini kaydet
    /// </summary>
    public void RecordLogout()
    {
        LogoutAt = DateTime.UtcNow;

        if (LoginAt < LogoutAt)
        {
            SessionDurationMinutes = (int)(LogoutAt.Value - LoginAt).TotalMinutes;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Oturum aktif mi kontrol et
    /// </summary>
    public bool IsSessionActive(int sessionTimeoutMinutes = 30)
    {
        if (LogoutAt.HasValue)
            return false;

        var sessionExpiry = LoginAt.AddMinutes(sessionTimeoutMinutes);
        return DateTime.UtcNow < sessionExpiry;
    }
}