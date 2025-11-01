using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.Application.Abstractions;
using Identity.Application.Extensions;
using Identity.Domain.Aggregates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
namespace Shared.Infrastructure.Services;
public class JwtTokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<JwtTokenService> _logger;
    private readonly SymmetricSecurityKey _securityKey;
    public JwtTokenService(IOptions<JwtOptions> jwtOptions, ILogger<JwtTokenService> logger)
    {
        if (jwtOptions?.Value == null)
            throw new ArgumentNullException(nameof(jwtOptions), "JwtOptions cannot be null");
        _jwtOptions = jwtOptions.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        if (string.IsNullOrWhiteSpace(_jwtOptions.Key))
            throw new ArgumentException("JWT Key cannot be empty", nameof(jwtOptions));
        if (string.IsNullOrWhiteSpace(_jwtOptions.Issuer))
            throw new ArgumentException("JWT Issuer cannot be empty", nameof(jwtOptions));
        if (string.IsNullOrWhiteSpace(_jwtOptions.Audience))
            throw new ArgumentException("JWT Audience cannot be empty", nameof(jwtOptions));
        if (_jwtOptions.Key.Length < 32)
            _logger.LogWarning(
                "JWT Key is less than 32 bytes. Recommended minimum length is 256 bits (32 bytes)");
        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        _logger.LogInformation("JwtTokenService initialized successfully");
    }
    public int AccessTokenExpirationSeconds => _jwtOptions.AccessTokenExpirationMinutes * 60;
    public string GenerateAccessToken(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "User cannot be null");
        try
        {
            _logger.LogDebug("Generating access token for user: {UserId} ({Email})", user.Id, user.Email);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new("IsActive", user.IsActive.ToString().ToLower()),
                new("IsLocked", user.IsLocked.ToString().ToLower())
            };
            if (user.Roles?.Any() == true)
                foreach (var userRole in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole.RoleName));
                    if (userRole.Permissions?.Any() == true)
                        foreach (var rolePermission in userRole.Permissions)
                            claims.Add(new Claim("permission", rolePermission.PermissionName));
                }
            var credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt,
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                SigningCredentials = credentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            _logger.LogDebug(
                "Access token generated successfully for user: {UserId}. Expires at: {ExpiresAt}",
                user.Id, expiresAt);
            return tokenString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating access token for user: {UserId}", user.Id);
            throw;
        }
    }
    public string GenerateRefreshToken()
    {
        try
        {
            _logger.LogDebug("Generating new refresh token");
            var randomBytes = new byte[64];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            var refreshToken = Convert.ToBase64String(randomBytes);
            _logger.LogDebug("Refresh token generated successfully");
            return refreshToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating refresh token");
            throw;
        }
    }
    public ClaimsPrincipal ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty", nameof(token));
        try
        {
            _logger.LogDebug("Validating token");
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _securityKey,
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            _logger.LogDebug(
                "Token validated successfully. UserId: {UserId}",
                principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return principal;
        }
        catch (SecurityTokenExpiredException ex)
        {
            _logger.LogWarning("Token validation failed: Token expired");
            throw new ArgumentException("Token has expired", nameof(token), ex);
        }
        catch (SecurityTokenInvalidSignatureException ex)
        {
            _logger.LogWarning("Token validation failed: Invalid signature");
            throw new ArgumentException("Token has invalid signature", nameof(token), ex);
        }
        catch (SecurityTokenInvalidIssuerException ex)
        {
            _logger.LogWarning("Token validation failed: Invalid issuer");
            throw new ArgumentException("Token has invalid issuer", nameof(token), ex);
        }
        catch (SecurityTokenInvalidAudienceException ex)
        {
            _logger.LogWarning("Token validation failed: Invalid audience");
            throw new ArgumentException("Token has invalid audience", nameof(token), ex);
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogWarning("Token validation failed: {Message}", ex.Message);
            throw new ArgumentException("Token validation failed", nameof(token), ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error validating token");
            throw;
        }
    }
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string expiredToken)
    {
        if (string.IsNullOrWhiteSpace(expiredToken))
        {
            _logger.LogWarning("GetPrincipalFromExpiredToken called with null or empty token");
            return null;
        }
        try
        {
            _logger.LogDebug("Extracting principal from expired token");
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _securityKey,
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtOptions.Audience,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };
            var principal = tokenHandler.ValidateToken(expiredToken, validationParameters, out _);
            _logger.LogDebug(
                "Principal extracted from expired token. UserId: {UserId}",
                principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return principal;
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogWarning(
                ex,
                "Failed to extract principal from expired token: {Message}",
                ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error extracting principal from expired token");
            return null;
        }
    }
}