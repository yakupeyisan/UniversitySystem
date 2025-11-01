namespace Identity.Application.DTOs;
public class ForgotPasswordRequest
{
    public ForgotPasswordRequest()
    {
    }
    public ForgotPasswordRequest(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        Email = email.Trim().ToLower();
    }
    public string Email { get; set; } = string.Empty;
}