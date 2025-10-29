using Identity.Domain.Aggregates;
using Identity.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Persistence.Contexts;

namespace Shared.Infrastructure.Persistence.Repositories.Identity;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var spec = new UserByEmailSpecification(email);
        return await GetAsync(spec, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .AnyAsync(u => u.Email.Value.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<List<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        var spec = new ActiveUsersSpecification();
        var result = await GetAllAsync(spec, cancellationToken);
        return result.ToList();
    }

    public async Task<List<User>> GetByRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var spec = new UsersByRoleSpecification(roleId);
        var result = await GetAllAsync(spec, cancellationToken);
        return result.ToList();
    }

    public async Task<List<User>> GetLockedUsersAsync(CancellationToken cancellationToken = default)
    {
        var spec = new LockedUsersSpecification();
        var result = await GetAllAsync(spec, cancellationToken);
        return result.ToList();
    }

    public async Task<User> GetWithRolesAndPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var spec = new UserByIdSpecification(userId);
        return await GetAsync(spec, cancellationToken);
    }
}