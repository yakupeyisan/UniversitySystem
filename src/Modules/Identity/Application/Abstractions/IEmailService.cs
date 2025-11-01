namespace Identity.Application.Abstractions;
public interface IEmailService
{
    Task SendVerificationEmailAsync(string email, string verificationCode,
    CancellationToken cancellationToken = default);
    Task SendPasswordResetEmailAsync(string email, string resetToken, CancellationToken cancellationToken = default);
    Task SendLoginNotificationAsync(string email, string ipAddress, string userAgent,
    CancellationToken cancellationToken = default);
    Task SendAccountLockedEmailAsync(string email, CancellationToken cancellationToken = default);
}