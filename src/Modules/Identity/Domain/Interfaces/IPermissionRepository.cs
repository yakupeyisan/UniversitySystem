using Core.Domain.Repositories;
using Core.Domain.Specifications;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Interfaces;

public interface IPermissionRepository : IRepository<Permission>
{
    Task<Permission> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Permission>> GetActivePermissionsAsync(CancellationToken cancellationToken = default);
    Task<List<Permission>> GetBySpecificationAsync(ISpecification<Permission> spec, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(ISpecification<Permission> spec, CancellationToken cancellationToken = default);
}