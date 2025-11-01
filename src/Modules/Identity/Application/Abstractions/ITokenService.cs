using System.Security.Claims;
using Identity.Domain.Aggregates;
namespace Identity.Application.Abstractions;
public interface ITokenService
{
    int AccessTokenExpirationSeconds { get; }
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal ValidateToken(string token);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string expiredToken);
}