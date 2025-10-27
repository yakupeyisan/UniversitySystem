namespace Identity.Application.Abstractions;

/// <summary>
/// Email service interface for verification and notifications
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends email verification code to user
    /// </summary>
    Task SendVerificationEmailAsync(string email, string verificationCode);

    /// <summary>
    /// Sends password reset email
    /// </summary>
    Task SendPasswordResetEmailAsync(string email, string resetToken);

    /// <summary>
    /// Sends login notification
    /// </summary>
    Task SendLoginNotificationAsync(string email, string ipAddress, string userAgent);

    /// <summary>
    /// Sends account locked notification
    /// </summary>
    Task SendAccountLockedEmailAsync(string email);
}