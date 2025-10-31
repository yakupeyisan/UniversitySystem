namespace Identity.Application.DTOs;

public class AssignPermissionToRoleRequest
{
    public AssignPermissionToRoleRequest()
    {
    }

    public AssignPermissionToRoleRequest(Guid permissionId)
    {
        if (permissionId == Guid.Empty)
            throw new ArgumentException("Permission ID cannot be empty", nameof(permissionId));

        PermissionId = permissionId;
    }

    public Guid PermissionId { get; set; }
}

/// <summary>
/// 2FA Kurulum DTO
/// </summary>
public class TwoFactorSetupDto
{
    public Guid UserId { get; set; }
    public string Method { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string QRCodeData { get; set; } = string.Empty;
    public List<string> BackupCodes { get; set; } = new();
    public string SetupInstructions { get; set; } = string.Empty;
}

/// <summary>
/// Giriþ Geçmiþi DTO
/// </summary>
public class LoginHistoryDto
{
    public Guid Id { get; set; }
    public DateTime LoginAt { get; set; }
    public DateTime? LogoutAt { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string BrowserName { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public bool IsTwoFactorUsed { get; set; }
    public int? SessionDurationMinutes { get; set; }
}

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

/// <summary>
/// 2FA Durumu DTO
/// </summary>
public class TwoFactorStatusDto
{
    public Guid UserId { get; set; }
    public bool IsEnabled { get; set; }
    public string Method { get; set; } = string.Empty;
    public DateTime? EnabledAt { get; set; }
    public int RemainingBackupCodes { get; set; }
}

/// <summary>
/// Baþarýsýz Giriþ Denemesi DTO
/// </summary>
public class FailedLoginAttemptDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string ErrorReason { get; set; } = string.Empty;
    public DateTime AttemptedAt { get; set; }
    public string Location { get; set; } = string.Empty;
}