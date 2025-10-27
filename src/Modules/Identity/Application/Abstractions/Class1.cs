using System.Security.Claims;
using Identity.Domain.Aggregates;

namespace Identity.Application.Abstractions;

/// <summary>
/// Token generation and validation service interface
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT access token for the user
    /// </summary>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Generates a refresh token
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Validates a JWT token and returns the principal
    /// </summary>
    ClaimsPrincipal ValidateToken(string token);

    /// <summary>
    /// Gets the expiration time for access tokens in seconds
    /// </summary>
    int AccessTokenExpirationSeconds { get; }
}

/// <summary>
/// Password hashing and verification service interface
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a password using secure algorithm
    /// Returns tuple of (HashedPassword, Salt)
    /// </summary>
    (string HashedPassword, string Salt) HashPassword(string password);

    /// <summary>
    /// Verifies a password against its hash
    /// </summary>
    bool VerifyPassword(string password, string hashedPassword, string salt);

    /// <summary>
    /// Validates password strength
    /// </summary>
    bool ValidatePasswordStrength(string password);

    /// <summary>
    /// Gets password strength requirements message
    /// </summary>
    string GetPasswordRequirements();
}

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