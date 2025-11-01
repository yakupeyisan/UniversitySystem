namespace Identity.Application.DTOs;

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