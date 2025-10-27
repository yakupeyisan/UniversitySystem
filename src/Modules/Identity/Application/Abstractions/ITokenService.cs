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