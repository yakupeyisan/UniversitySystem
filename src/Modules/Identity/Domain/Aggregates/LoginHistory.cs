using Core.Domain;
using Identity.Domain.Enums;

namespace Identity.Domain.Aggregates;

/// <summary>
/// Kullanýcý giriþ geçmiþi - Audit log ve güvenlik analizi için
/// </summary>
public class LoginHistory : AuditableEntity
{
    private LoginHistory()
    {
    }

    /// <summary>
    /// Kullanýcý ID
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Giriþ tarihi ve saati
    /// </summary>
    public DateTime LoginAt { get; private set; }

    /// <summary>
    /// Çýkýþ tarihi ve saati (logout)
    /// </summary>
    public DateTime? LogoutAt { get; private set; }

    /// <summary>
    /// Istemci IP adresi
    /// </summary>
    public string IpAddress { get; private set; }

    /// <summary>
    /// User Agent (tarayýcý/cihaz bilgisi)
    /// </summary>
    public string UserAgent { get; private set; }

    /// <summary>
    /// Ýþletim Sistemi
    /// </summary>
    public string OperatingSystem { get; private set; }

    /// <summary>
    /// Tarayýcý adý
    /// </summary>
    public string BrowserName { get; private set; }

    /// <summary>
    /// Cihaz tipi (Desktop/Mobile/Tablet)
    /// </summary>
    public string DeviceType { get; private set; }

    /// <summary>
    /// Coðrafi konum (IP'den tahmin edilen)
    /// </summary>
    public string Location { get; private set; }

    /// <summary>
    /// Giriþ sonucu (Success/Failed/Locked)
    /// </summary>
    public LoginResultType Result { get; private set; }

    /// <summary>
    /// Hata mesajý (varsa)
    /// </summary>
    public string ErrorMessage { get; private set; }

    /// <summary>
    /// Oturum süresi (dakika cinsinden)
    /// </summary>
    public int? SessionDurationMinutes { get; private set; }

    /// <summary>
    /// 2FA kullanýldý mý
    /// </summary>
    public bool IsTwoFactorUsed { get; private set; }

    /// <summary>
    /// Access token oluþturuldu mý
    /// </summary>
    public bool AccessTokenCreated { get; private set; }

    /// <summary>
    /// Refresh token oluþturuldu mý
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
    /// Çýkýþ iþlemini kaydet
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