using Core.Domain.Pagination;
using Core.Domain.Specifications;
namespace Core.Domain.Repositories;
public interface IGenericRepository<TEntity> where TEntity : Entity
{
    Task<TEntity?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken = default);
    Task<TEntity?> GetAsync(
    ISpecification<TEntity> specification,
    CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(
    CancellationToken cancellationToken = default);
    Task<PagedList<TEntity>> GetAllAsync(
    PagedRequest pagedRequest,
    CancellationToken cancellationToken = default);
    Task<PagedList<TEntity>> GetAllAsync(
    ISpecification<TEntity> specification,
    PagedRequest pagedRequest,
    CancellationToken cancellationToken = default);
    Task<PagedList<TEntity>> GetAllAsync(
    ISpecification<TEntity> specification,
    CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(
    Guid id,
    CancellationToken cancellationToken = default);
    Task<int> CountAsync(
    CancellationToken cancellationToken = default);
    Task AddAsync(
    TEntity aggregate,
    CancellationToken cancellationToken = default);
    Task AddRangeAsync(
    IEnumerable<TEntity> aggregates,
    CancellationToken cancellationToken = default);
    Task UpdateAsync(
    TEntity aggregate,
    CancellationToken cancellationToken = default);
    Task DeleteAsync(
    TEntity aggregate,
    CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(
    IEnumerable<TEntity> aggregates,
    CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}