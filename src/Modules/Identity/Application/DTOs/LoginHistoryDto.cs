namespace Identity.Application.DTOs;
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