using Core.Domain;
using Identity.Domain.Enums;
using Identity.Domain.Events;
using MediatR;

namespace Identity.Domain.Aggregates;

/// <summary>
/// Kullanıcı hesabı kilitleme kaydı - Güvenlik için
/// </summary>
public class UserAccountLockout : AuditableEntity
{
    private UserAccountLockout()
    {
    }

    /// <summary>
    /// Kullanıcı ID
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Kilitleme nedeni
    /// </summary>
    public AccountLockoutReason Reason { get; private set; }

    /// <summary>
    /// Kilitleme başlangıç tarihi
    /// </summary>
    public DateTime LockedAt { get; private set; }

    /// <summary>
    /// Kilitleme bitiş tarihi (automatic unlock)
    /// </summary>
    public DateTime? LockedUntil { get; private set; }

    /// <summary>
    /// Süre tipі (minutes, hours, days)
    /// </summary>
    public LockoutDurationType DurationType { get; private set; }

    /// <summary>
    /// Süre değeri
    /// </summary>
    public int DurationValue { get; private set; }

    /// <summary>
    /// Kilitleme sebebinin detayları
    /// </summary>
    public string ReasonDetails { get; private set; }

    /// <summary>
    /// Başarısız deneme sayısı
    /// </summary>
    public int FailedAttemptCount { get; private set; }

    /// <summary>
    /// IP adresleri (virgülle ayrılmış)
    /// </summary>
    public string IpAddresses { get; private set; }

    /// <summary>
    /// Kilitlemelerden sonra açıldı mı
    /// </summary>
    public bool IsUnlocked { get; private set; }

    /// <summary>
    /// Açılış tarihi
    /// </summary>
    public DateTime? UnlockedAt { get; private set; }

    /// <summary>
    /// Açılış nedeni (Manual/Automatic)
    /// </summary>
    public string UnlockReason { get; private set; }

    /// <summary>
    /// Navigation property
    /// </summary>
    public User User { get; private set; }

    public static UserAccountLockout CreateFromFailedAttempts(
        Guid userId,
        int failedAttemptCount,
        int lockoutMinutes,
        string ipAddress,
        string details = "")
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (lockoutMinutes <= 0)
            throw new ArgumentException("Lockout minutes must be greater than 0", nameof(lockoutMinutes));

        return new UserAccountLockout
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Reason = AccountLockoutReason.TooManyFailedLoginAttempts,
            LockedAt = DateTime.UtcNow,
            LockedUntil = DateTime.UtcNow.AddMinutes(lockoutMinutes),
            DurationType = LockoutDurationType.Minutes,
            DurationValue = lockoutMinutes,
            ReasonDetails = details ?? string.Empty,
            FailedAttemptCount = failedAttemptCount,
            IpAddresses = ipAddress ?? string.Empty,
            IsUnlocked = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static UserAccountLockout CreateManual(
        Guid userId,
        string reason,
        int durationMinutes = 1440) // Default: 24 hours
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        return new UserAccountLockout
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Reason = AccountLockoutReason.AdminLocked,
            LockedAt = DateTime.UtcNow,
            LockedUntil = DateTime.UtcNow.AddMinutes(durationMinutes),
            DurationType = LockoutDurationType.Minutes,
            DurationValue = durationMinutes,
            ReasonDetails = reason ?? string.Empty,
            FailedAttemptCount = 0,
            IsUnlocked = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Hesabı aç
    /// </summary>
    public void Unlock(string reason = "")
    {
        IsUnlocked = true;
        UnlockedAt = DateTime.UtcNow;
        UnlockReason = reason ?? "Manually unlocked";
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new UserUnlockedEvent(Id, UnlockReason));
    }

    /// <summary>
    /// Kilitleme aktif mi kontrol et
    /// </summary>
    public bool IsActiveLockout()
    {
        if (IsUnlocked)
            return false;

        if (LockedUntil.HasValue && DateTime.UtcNow > LockedUntil.Value)
        {
            // Otomatik unlock - süresi doldu
            return false;
        }

        return true;
    }

    /// <summary>
    /// Kilitleme kaç dakika kaldığını döndür
    /// </summary>
    public int GetRemainingLockoutMinutes()
    {
        if (IsUnlocked || !LockedUntil.HasValue)
            return 0;

        var remaining = (int)(LockedUntil.Value - DateTime.UtcNow).TotalMinutes;
        return remaining > 0 ? remaining : 0;
    }
}