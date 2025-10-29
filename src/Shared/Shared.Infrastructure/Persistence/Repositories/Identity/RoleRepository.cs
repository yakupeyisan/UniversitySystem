using Core.Domain.Specifications;
using Identity.Domain.Aggregates;
using Identity.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Persistence.Contexts;

namespace Shared.Infrastructure.Persistence.Repositories.Identity;

public class RoleRepository : GenericRepository<Role>
{
    private readonly AppDbContext _context;

    public RoleRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Role> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var spec = new RoleByNameSpecification(name);
        return await GetAsync(spec, cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .AnyAsync(r => r.RoleName.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task<List<Role>> GetActiveRolesAsync(CancellationToken cancellationToken = default)
    {
        var spec = new ActiveRolesSpecification();
        var result = await GetAllAsync(spec, cancellationToken);
        return result.ToList();
    }

    public async Task<Role> GetWithPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var spec = new RoleByIdSpecification(roleId);
        return await GetAsync(spec, cancellationToken);
    }

    public async Task<List<Role>> GetBySpecificationAsync(ISpecification<Role> spec,
        CancellationToken cancellationToken = default)
    {
        var result = await GetAllAsync(spec, cancellationToken);
        return result.ToList();
    }
}