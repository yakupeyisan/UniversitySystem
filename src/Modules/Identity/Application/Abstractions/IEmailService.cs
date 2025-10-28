namespace Identity.Application.Abstractions;

/// <summary>
/// Email service interface for verification and notifications
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends email verification code to user
    /// </summary>
    Task SendVerificationEmailAsync(string email, string verificationCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends password reset email
    /// </summary>
    Task SendPasswordResetEmailAsync(string email, string resetToken,CancellationToken cancellationToken=default);

    /// <summary>
    /// Sends login notification
    /// </summary>
    Task SendLoginNotificationAsync(string email, string ipAddress, string userAgent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends account locked notification
    /// </summary>
    Task SendAccountLockedEmailAsync(string email, CancellationToken cancellationToken = default);
}