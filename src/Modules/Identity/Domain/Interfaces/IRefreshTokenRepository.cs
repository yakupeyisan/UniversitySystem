using Core.Domain.Repositories;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Interfaces;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<List<RefreshToken>> GetUserTokensAsync(Guid userId, CancellationToken cancellationToken = default);
    Task RemoveExpiredTokensAsync(CancellationToken cancellationToken = default);
}