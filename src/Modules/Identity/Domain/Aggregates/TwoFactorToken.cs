using Core.Domain;
using Identity.Domain.Enums;

namespace Identity.Domain.Aggregates;

/// <summary>
/// 2FA TOTP token entity - Authenticator app taraf�ndan olu�turulan kodlar
/// </summary>
public class TwoFactorToken : AuditableEntity
{
    private TwoFactorToken()
    {
    }

    /// <summary>
    /// Kullan�c� ID
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// 2FA y�ntemi (Google Authenticator, Microsoft Authenticator, SMS, Email)
    /// </summary>
    public TwoFactorMethod Method { get; private set; }

    /// <summary>
    /// Gizli anahtar (secret key) - TOTP �retimi i�in
    /// </summary>
    public string SecretKey { get; private set; }

    /// <summary>
    /// Backup kodlar� (e.g., "ABCD-EFGH-IJKL-MNOP" format�nda, pipe ile ayr�lm��)
    /// </summary>
    public string BackupCodes { get; private set; }

    /// <summary>
    /// Token'�n aktif olup olmad���
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Token'�n do�rulan�p do�rulanmad��� (kurulum s�ras�nda test edilir)
    /// </summary>
    public bool IsVerified { get; private set; }

    /// <summary>
    /// Do�rulama tarihi
    /// </summary>
    public DateTime? VerifiedAt { get; private set; }

    /// <summary>
    /// Son ba�ar�l� do�rulama tarihi
    /// </summary>
    public DateTime? LastUsedAt { get; private set; }

    /// <summary>
    /// Devre d��� b�rak�lma tarihi (None varsa)
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
    /// Token'� do�rula ve aktifle�tir
    /// </summary>
    public void Verify()
    {
        IsVerified = true;
        IsActive = true;
        VerifiedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Son kullan�m zaman�n� g�ncelle
    /// </summary>
    public void UpdateLastUsedTime()
    {
        LastUsedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Token'� devre d��� b�rak
    /// </summary>
    public void Disable()
    {
        IsActive = false;
        DisabledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Yedek kodlar� al
    /// </summary>
    public List<string> GetBackupCodes()
    {
        return BackupCodes
            .Split('|', StringSplitOptions.RemoveEmptyEntries)
            .Select(code => code.Trim())
            .ToList();
    }

    /// <summary>
    /// Backup kod kullan�ld���nda g�ncelle
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