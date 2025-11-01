namespace Identity.Application.DTOs;

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