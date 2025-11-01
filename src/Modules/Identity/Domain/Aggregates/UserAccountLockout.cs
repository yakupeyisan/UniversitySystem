using Core.Domain;
using Identity.Domain.Enums;
using Identity.Domain.Events;
using MediatR;
namespace Identity.Domain.Aggregates;
public class UserAccountLockout : AuditableEntity
{
    private UserAccountLockout()
    {
    }
    public Guid UserId { get; private set; }
    public AccountLockoutReason Reason { get; private set; }
    public DateTime LockedAt { get; private set; }
    public DateTime? LockedUntil { get; private set; }
    public LockoutDurationType DurationType { get; private set; }
    public int DurationValue { get; private set; }
    public string ReasonDetails { get; private set; }
    public int FailedAttemptCount { get; private set; }
    public string IpAddresses { get; private set; }
    public bool IsUnlocked { get; private set; }
    public DateTime? UnlockedAt { get; private set; }
    public string UnlockReason { get; private set; }
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
        int durationMinutes = 1440)
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
    public void Unlock(string reason = "")
    {
        IsUnlocked = true;
        UnlockedAt = DateTime.UtcNow;
        UnlockReason = reason ?? "Manually unlocked";
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new UserUnlockedEvent(Id, UnlockReason));
    }
    public bool IsActiveLockout()
    {
        if (IsUnlocked)
            return false;
        if (LockedUntil.HasValue && DateTime.UtcNow > LockedUntil.Value)
        {
            return false;
        }
        return true;
    }
    public int GetRemainingLockoutMinutes()
    {
        if (IsUnlocked || !LockedUntil.HasValue)
            return 0;
        var remaining = (int)(LockedUntil.Value - DateTime.UtcNow).TotalMinutes;
        return remaining > 0 ? remaining : 0;
    }
}