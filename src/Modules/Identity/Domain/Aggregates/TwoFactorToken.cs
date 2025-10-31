using Core.Domain;
using Identity.Domain.Enums;

namespace Identity.Domain.Aggregates;

/// <summary>
/// 2FA TOTP token entity - Authenticator app tarafýndan oluþturulan kodlar
/// </summary>
public class TwoFactorToken : AuditableEntity
{
    private TwoFactorToken()
    {
    }

    /// <summary>
    /// Kullanýcý ID
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// 2FA yöntemi (Google Authenticator, Microsoft Authenticator, SMS, Email)
    /// </summary>
    public TwoFactorMethod Method { get; private set; }

    /// <summary>
    /// Gizli anahtar (secret key) - TOTP üretimi için
    /// </summary>
    public string SecretKey { get; private set; }

    /// <summary>
    /// Backup kodlarý (e.g., "ABCD-EFGH-IJKL-MNOP" formatýnda, pipe ile ayrýlmýþ)
    /// </summary>
    public string BackupCodes { get; private set; }

    /// <summary>
    /// Token'ýn aktif olup olmadýðý
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Token'ýn doðrulanýp doðrulanmadýðý (kurulum sýrasýnda test edilir)
    /// </summary>
    public bool IsVerified { get; private set; }

    /// <summary>
    /// Doðrulama tarihi
    /// </summary>
    public DateTime? VerifiedAt { get; private set; }

    /// <summary>
    /// Son baþarýlý doðrulama tarihi
    /// </summary>
    public DateTime? LastUsedAt { get; private set; }

    /// <summary>
    /// Devre dýþý býrakýlma tarihi (None varsa)
    /// </summary>
    public DateTime? DisabledAt { get; private set; }

    /// <summary>
    /// Navigation property
    /// </summary>
    public User User { get; private set; }

    public static TwoFactorToken Create(
        Guid userId,
        TwoFactorMethod method,
        string secretKey,
        string backupCodes)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (string.IsNullOrWhiteSpace(secretKey))
            throw new ArgumentException("Secret key cannot be empty", nameof(secretKey));

        if (string.IsNullOrWhiteSpace(backupCodes))
            throw new ArgumentException("Backup codes cannot be empty", nameof(backupCodes));

        return new TwoFactorToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Method = method,
            SecretKey = secretKey,
            BackupCodes = backupCodes,
            IsActive = false,
            IsVerified = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Token'ý doðrula ve aktifleþtir
    /// </summary>
    public void Verify()
    {
        IsVerified = true;
        IsActive = true;
        VerifiedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Son kullaným zamanýný güncelle
    /// </summary>
    public void UpdateLastUsedTime()
    {
        LastUsedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Token'ý devre dýþý býrak
    /// </summary>
    public void Disable()
    {
        IsActive = false;
        DisabledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Yedek kodlarý al
    /// </summary>
    public List<string> GetBackupCodes()
    {
        return BackupCodes
            .Split('|', StringSplitOptions.RemoveEmptyEntries)
            .Select(code => code.Trim())
            .ToList();
    }

    /// <summary>
    /// Backup kod kullanýldýðýnda güncelle
    /// </summary>
    public void UseBackupCode(string code)
    {
        var codes = GetBackupCodes();
        if (!codes.Contains(code))
            throw new InvalidOperationException("Backup code not found");

        codes.Remove(code);
        BackupCodes = string.Join("|", codes);
        UpdatedAt = DateTime.UtcNow;
    }
}