namespace Identity.Application.DTOs;

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