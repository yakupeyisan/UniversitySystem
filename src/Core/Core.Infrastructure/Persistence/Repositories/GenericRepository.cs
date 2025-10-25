using Core.Domain;
using Core.Domain.Pagination;
using Core.Domain.Repositories;
using Core.Domain.Specifications;
using Core.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
namespace Core.Infrastructure.Persistence.Repositories;
public class GenericRepository<TAggregate> : IGenericRepository<TAggregate>
    where TAggregate : AggregateRoot
{
    protected readonly AppDbContext _context;
    public GenericRepository(AppDbContext context)
    {
        _context = context;
    }
    #region Read Operations
    public async Task<TAggregate?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<TAggregate>().FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<TAggregate?> GetAsync(
        ISpecification<TAggregate> specification,
        CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<IEnumerable<TAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<TAggregate>().ToListAsync(cancellationToken);
    }
    public async Task<PagedList<TAggregate>> GetAllAsync(
        PagedRequest pagedRequest,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await _context.Set<TAggregate>().CountAsync(cancellationToken);
        var items = await _context.Set<TAggregate>()
            .Skip((pagedRequest.PageNumber - 1) * pagedRequest.PageSize)
            .Take(pagedRequest.PageSize)
            .ToListAsync(cancellationToken);
        return new PagedList<TAggregate>(items, totalCount, pagedRequest.PageNumber, pagedRequest.PageSize);
    }
    public async Task<PagedList<TAggregate>> GetAllAsync(
        ISpecification<TAggregate> specification,
        PagedRequest pagedRequest,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagedRequest.PageNumber - 1) * pagedRequest.PageSize)
            .Take(pagedRequest.PageSize)
            .ToListAsync(cancellationToken);
        return new PagedList<TAggregate>(items, totalCount, pagedRequest.PageNumber, pagedRequest.PageSize);
    }
    public async Task<PagedList<TAggregate>> GetAllAsync(
        ISpecification<TAggregate> specification,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.ToListAsync(cancellationToken);
        return new PagedList<TAggregate>(items, totalCount, 1, totalCount);
    }
    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<TAggregate>().AnyAsync(e => e.Id == id, cancellationToken);
    }
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<TAggregate>().CountAsync(cancellationToken);
    }
    #endregion
    #region Write Operations
    public async Task AddAsync(
        TAggregate aggregate,
        CancellationToken cancellationToken = default)
    {
        await _context.Set<TAggregate>().AddAsync(aggregate, cancellationToken);
    }
    public async Task AddRangeAsync(
        IEnumerable<TAggregate> aggregates,
        CancellationToken cancellationToken = default)
    {
        await _context.Set<TAggregate>().AddRangeAsync(aggregates, cancellationToken);
    }
    public async Task UpdateAsync(
        TAggregate aggregate,
        CancellationToken cancellationToken = default)
    {
        _context.Set<TAggregate>().Update(aggregate);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(
        TAggregate aggregate,
        CancellationToken cancellationToken = default)
    {
        _context.Set<TAggregate>().Remove(aggregate);
        await Task.CompletedTask;
    }
    public async Task DeleteRangeAsync(
        IEnumerable<TAggregate> aggregates,
        CancellationToken cancellationToken = default)
    {
        _context.Set<TAggregate>().RemoveRange(aggregates);
        await Task.CompletedTask;
    }
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
    #endregion
    #region Specification Helper
    protected IQueryable<TAggregate> ApplySpecification(ISpecification<TAggregate> spec)
    {
        var query = _context.Set<TAggregate>().AsQueryable();
        if (spec.IsSplitQuery)
            query = query.AsSplitQuery();
        if (spec.Criteria != null)
            query = query.Where(spec.Criteria);
        if (spec.Includes != null && spec.Includes.Any())
        {
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
        }
        if (spec.IncludeStrings != null && spec.IncludeStrings.Any())
        {
            query = spec.IncludeStrings.Aggregate(query, (current, includeString) => current.Include(includeString));
        }
        if (spec.OrderBys != null && spec.OrderBys.Any())
        {
            foreach (var (orderExpression, isDescending) in spec.OrderBys)
            {
                query = isDescending
                    ? query.OrderByDescending(orderExpression)
                    : query.OrderBy(orderExpression);
            }
        }
        if (spec.IsPagingEnabled)
        {
            if (spec.Skip.HasValue)
                query = query.Skip(spec.Skip.Value);
            if (spec.Take.HasValue)
                query = query.Take(spec.Take.Value);
        }
        return query;
    }
    #endregion
}