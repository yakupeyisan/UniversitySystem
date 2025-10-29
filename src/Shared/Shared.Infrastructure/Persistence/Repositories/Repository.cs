using AutoMapper;
using Core.Domain;
using Core.Domain.Pagination;
using Core.Domain.Repositories;
using Core.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Persistence.Contexts;

namespace Shared.Infrastructure.Persistence.Repositories;

public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : Entity
{
    protected readonly AppDbContext _context;
    protected readonly IMapper _mapper;

    public Repository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    #region Specification Helper

    protected IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
    {
        var query = _context.Set<TEntity>().AsQueryable();
        if (spec.IsSplitQuery)
            query = query.AsSplitQuery();
        if (spec.Criteria != null)
            query = query.Where(spec.Criteria);
        if (spec.Includes != null && spec.Includes.Any())
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
        if (spec.IncludeStrings != null && spec.IncludeStrings.Any())
            query = spec.IncludeStrings.Aggregate(query, (current, includeString) => current.Include(includeString));
        if (spec.OrderBys != null && spec.OrderBys.Any())
            foreach (var (orderExpression, isDescending) in spec.OrderBys)
                query = isDescending
                    ? query.OrderByDescending(orderExpression)
                    : query.OrderBy(orderExpression);

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

    #region Read Operations

    public async Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<TMap?> GetByIdAsync<TMap>(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await GetByIdAsync(id, cancellationToken);
        return _mapper.Map<TMap>(result);
    }

    public async Task<TEntity?> GetAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TMap?> GetAsync<TMap>(ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var result = await GetAsync(specification, cancellationToken);
        return _mapper.Map<TMap>(result);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<TEntity>().ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TMap>> GetAllAsync<TMap>(CancellationToken cancellationToken = default)
    {
        var results = await GetAllAsync(cancellationToken);
        var mapperResults = results.Select(x => _mapper.Map<TMap>(x));
        return mapperResults;
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

    public async Task<PagedList<TMap>> GetAllAsync<TMap>(PagedRequest pagedRequest,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await _context.Set<TEntity>().CountAsync(cancellationToken);
        var items = await _context.Set<TEntity>()
            .Skip((pagedRequest.PageNumber - 1) * pagedRequest.PageSize)
            .Take(pagedRequest.PageSize)
            .ToListAsync(cancellationToken);
        var mappedItems = items.Select(x => _mapper.Map<TMap>(x));
        return new PagedList<TMap>(mappedItems, totalCount, pagedRequest.PageNumber, pagedRequest.PageSize);
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

    public async Task<PagedList<TMap>> GetAllAsync<TMap>(ISpecification<TEntity> specification,
        PagedRequest pagedRequest,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagedRequest.PageNumber - 1) * pagedRequest.PageSize)
            .Take(pagedRequest.PageSize)
            .ToListAsync(cancellationToken);
        var mappedItems = items.Select(x => _mapper.Map<TMap>(x));
        return new PagedList<TMap>(mappedItems, totalCount, pagedRequest.PageNumber, pagedRequest.PageSize);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TMap>> GetAllAsync<TMap>(ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var results = await GetAllAsync(specification, cancellationToken);
        var mapperResults = results.Select(x => _mapper.Map<TMap>(x));
        return mapperResults;
    }

    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<TEntity>().AnyAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> IsUniqueAsync(ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(spec);
        return await query.CountAsync(cancellationToken);
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
}