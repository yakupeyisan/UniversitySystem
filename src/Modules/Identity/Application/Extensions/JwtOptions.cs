namespace Identity.Application.Extensions;

/// <summary>
/// JWT configuration options
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// Secret key for signing JWT tokens
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Issuer of the JWT token
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Audience of the JWT token
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Access token expiration time in minutes (default: 60)
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Refresh token expiration time in days (default: 7)
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 7;
}