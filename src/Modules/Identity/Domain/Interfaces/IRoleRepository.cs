using Core.Domain.Repositories;
using Core.Domain.Specifications;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Interfaces;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Role>> GetActiveRolesAsync(CancellationToken cancellationToken = default);
    Task<Role> GetWithPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<List<Role>> GetBySpecificationAsync(ISpecification<Role> spec, CancellationToken cancellationToken = default);
}