namespace Identity.Application.DTOs;

public class VerifyEmailRequest
{
    public string VerificationCode { get; set; } = string.Empty;

    public VerifyEmailRequest() { }

    public VerifyEmailRequest(string verificationCode)
    {
        if (string.IsNullOrWhiteSpace(verificationCode))
            throw new ArgumentException("Verification code cannot be empty", nameof(verificationCode));

        VerificationCode = verificationCode.Trim();
    }
}