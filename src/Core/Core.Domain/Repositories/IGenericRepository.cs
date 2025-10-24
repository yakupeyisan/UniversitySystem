using Core.Domain.Pagination;
using Core.Domain.Specifications;
namespace Core.Domain.Repositories;
public interface IGenericRepository<TAggregate> where TAggregate : AggregateRoot
{
    Task<TAggregate?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken = default);
    Task<TAggregate?> GetAsync(
    ISpecification<TAggregate> specification,
    CancellationToken cancellationToken = default);
    Task<IEnumerable<TAggregate>> GetAllAsync(
    CancellationToken cancellationToken = default);
    Task<PagedList<TAggregate>> GetAllAsync(
    PagedRequest pagedRequest,
    CancellationToken cancellationToken = default);
    Task<PagedList<TAggregate>> GetAllAsync(
    ISpecification<TAggregate> specification,
    PagedRequest pagedRequest,
    CancellationToken cancellationToken = default);
    Task<PagedList<TAggregate>> GetAllAsync(
    ISpecification<TAggregate> specification,
    CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(
    Guid id,
    CancellationToken cancellationToken = default);
    Task<int> CountAsync(
    CancellationToken cancellationToken = default);
    Task AddAsync(
    TAggregate aggregate,
    CancellationToken cancellationToken = default);
    Task AddRangeAsync(
    IEnumerable<TAggregate> aggregates,
    CancellationToken cancellationToken = default);
    Task UpdateAsync(
    TAggregate aggregate,
    CancellationToken cancellationToken = default);
    Task DeleteAsync(
    TAggregate aggregate,
    CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(
    IEnumerable<TAggregate> aggregates,
    CancellationToken cancellationToken = default);
}