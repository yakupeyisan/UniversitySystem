using Core.Domain;
using Identity.Domain.Enums;
namespace Identity.Domain.Aggregates;
public class TwoFactorToken : AuditableEntity
{
    private TwoFactorToken()
    {
    }
    public Guid UserId { get; private set; }
    public TwoFactorMethod Method { get; private set; }
    public string SecretKey { get; private set; }
    public string BackupCodes { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsVerified { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public DateTime? LastUsedAt { get; private set; }
    public DateTime? DisabledAt { get; private set; }
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
    public void Verify()
    {
        IsVerified = true;
        IsActive = true;
        VerifiedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    public void UpdateLastUsedTime()
    {
        LastUsedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Disable()
    {
        IsActive = false;
        DisabledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    public List<string> GetBackupCodes()
    {
        return BackupCodes
            .Split('|', StringSplitOptions.RemoveEmptyEntries)
            .Select(code => code.Trim())
            .ToList();
    }
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