using Core.Domain.Repositories;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetByEmailAsync(string email,CancellationToken cancellationToken=default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<List<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
    Task<List<User>> GetByRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<List<User>> GetLockedUsersAsync(CancellationToken cancellationToken = default);
    Task<User> GetWithRolesAndPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface IRoleRepository : IRepository<Role>
{
    Task<Role> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Role>> GetActiveRolesAsync(CancellationToken cancellationToken = default);
    Task<Role> GetWithPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);
}

public interface IPermissionRepository : IRepository<Permission>
{
    Task<Permission> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Permission>> GetActivePermissionsAsync(CancellationToken cancellationToken = default);
}

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<List<RefreshToken>> GetUserTokensAsync(Guid userId, CancellationToken cancellationToken = default);
    Task RemoveExpiredTokensAsync(CancellationToken cancellationToken = default);
}