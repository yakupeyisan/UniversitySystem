using Core.Domain.Repositories;
using Core.Domain.Specifications;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<List<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
    Task<List<User>> GetByRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<List<User>> GetLockedUsersAsync(CancellationToken cancellationToken = default);
    Task<User> GetWithRolesAndPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(ISpecification<User> spec, CancellationToken cancellationToken = default);
}