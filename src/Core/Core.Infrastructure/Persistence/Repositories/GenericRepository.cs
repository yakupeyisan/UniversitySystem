using Core.Domain;
using Core.Domain.Pagination;
using Core.Domain.Repositories;
using Core.Domain.Specifications;
using Core.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
namespace Core.Infrastructure.Persistence.Repositories;
public class GenericRepository<TEntity> : IGenericRepository<TEntity>
    where TEntity : Entity
{
    protected readonly AppDbContext _context;
    public GenericRepository(AppDbContext context)
    {
        _context = context;
    }
    #region Read Operations
    public async Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<TEntity?> GetAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<IEnumerable<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<TEntity>().ToListAsync(cancellationToken);
    }
    public async Task<PagedList<TEntity>> GetAllAsync(
        PagedRequest pagedRequest,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await _context.Set<TEntity>().CountAsync(cancellationToken);
        var items = await _context.Set<TEntity>()
            .Skip((pagedRequest.PageNumber - 1) * pagedRequest.PageSize)
            .Take(pagedRequest.PageSize)
            .ToListAsync(cancellationToken);
        return new PagedList<TEntity>(items, totalCount, pagedRequest.PageNumber, pagedRequest.PageSize);
    }
    public async Task<PagedList<TEntity>> GetAllAsync(
        ISpecification<TEntity> specification,
        PagedRequest pagedRequest,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagedRequest.PageNumber - 1) * pagedRequest.PageSize)
            .Take(pagedRequest.PageSize)
            .ToListAsync(cancellationToken);
        return new PagedList<TEntity>(items, totalCount, pagedRequest.PageNumber, pagedRequest.PageSize);
    }
    public async Task<PagedList<TEntity>> GetAllAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.ToListAsync(cancellationToken);
        return new PagedList<TEntity>(items, totalCount, 1, totalCount);
    }
    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<TEntity>().AnyAsync(e => e.Id == id, cancellationToken);
    }
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<TEntity>().CountAsync(cancellationToken);
    }
    #endregion
    #region Write Operations
    public async Task AddAsync(
        TEntity aggregate,
        CancellationToken cancellationToken = default)
    {
        await _context.Set<TEntity>().AddAsync(aggregate, cancellationToken);
    }
    public async Task AddRangeAsync(
        IEnumerable<TEntity> aggregates,
        CancellationToken cancellationToken = default)
    {
        await _context.Set<TEntity>().AddRangeAsync(aggregates, cancellationToken);
    }
    public async Task UpdateAsync(
        TEntity aggregate,
        CancellationToken cancellationToken = default)
    {
        _context.Set<TEntity>().Update(aggregate);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(
        TEntity aggregate,
        CancellationToken cancellationToken = default)
    {
        _context.Set<TEntity>().Remove(aggregate);
        await Task.CompletedTask;
    }
    public async Task DeleteRangeAsync(
        IEnumerable<TEntity> aggregates,
        CancellationToken cancellationToken = default)
    {
        _context.Set<TEntity>().RemoveRange(aggregates);
        await Task.CompletedTask;
    }
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
    #endregion
    #region Specification Helper
    protected IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
    {
        var query = _context.Set<TEntity>().AsQueryable();
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