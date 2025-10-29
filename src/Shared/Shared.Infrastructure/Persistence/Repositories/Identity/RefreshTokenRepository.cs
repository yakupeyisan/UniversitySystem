using Identity.Domain.Aggregates;
using Identity.Domain.Interfaces;
using Identity.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Persistence.Contexts;

namespace Shared.Infrastructure.Persistence.Repositories.Identity;

public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<RefreshToken> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .Where(rt => rt.Token == token)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<RefreshToken>> GetUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var spec = new RefreshTokensByUserSpecification(userId);
        var result = await GetAllAsync(spec, cancellationToken);
        return result.ToList();
    }

    public async Task RemoveExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        var expiredTokens = await _context.RefreshTokens
            .Where(rt => rt.ExpiryDate <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        if (expiredTokens.Any())
        {
            _context.RefreshTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}