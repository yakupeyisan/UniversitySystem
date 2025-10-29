namespace Identity.Application.DTOs;

public class ResetPasswordRequest
{
    public ResetPasswordRequest()
    {
    }

    public ResetPasswordRequest(string email, string resetCode, string newPassword, string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        if (string.IsNullOrWhiteSpace(resetCode))
            throw new ArgumentException("Reset code cannot be empty", nameof(resetCode));
        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("Password cannot be empty", nameof(newPassword));
        if (newPassword != confirmPassword)
            throw new ArgumentException("Passwords do not match", nameof(confirmPassword));

        Email = email.Trim().ToLower();
        ResetCode = resetCode.Trim();
        NewPassword = newPassword;
        ConfirmPassword = confirmPassword;
    }

    public string Email { get; set; } = string.Empty;
    public string ResetCode { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}