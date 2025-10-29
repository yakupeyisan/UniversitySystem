using Core.Domain.Specifications;
using Identity.Domain.Aggregates;
using Identity.Domain.Interfaces;
using Identity.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Persistence.Contexts;

namespace Shared.Infrastructure.Persistence.Repositories.Identity;

public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
{
    private readonly AppDbContext _context;

    public PermissionRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Permission> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var spec = new PermissionByNameSpecification(name);
        return await GetAsync(spec, cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .AnyAsync(p => p.PermissionName.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task<List<Permission>> GetActivePermissionsAsync(CancellationToken cancellationToken = default)
    {
        var spec = new ActivePermissionsSpecification();
        var result = await GetAllAsync(spec, cancellationToken);
        return result.ToList();
    }

    public async Task<List<Permission>> GetBySpecificationAsync(ISpecification<Permission> spec, CancellationToken cancellationToken = default)
    {
        var result = await GetAllAsync(spec, cancellationToken);
        return result.ToList();
    }

}