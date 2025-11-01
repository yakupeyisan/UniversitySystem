namespace Identity.Application.DTOs;

/// <summary>
/// Hesap Kilitleme Durumu DTO
/// </summary>
public class AccountLockoutStatusDto
{
    public Guid UserId { get; set; }
    public bool IsLocked { get; set; }
    public bool IsCurrentlyLocked { get; set; }
    public DateTime? LockedUntil { get; set; }
    public int RemainingMinutes { get; set; }
    public string LockReason { get; set; } = string.Empty;
    public int FailedAttempts { get; set; }
    public int ActiveLockoutCount { get; set; }
}