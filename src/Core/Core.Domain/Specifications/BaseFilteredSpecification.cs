using System.Linq.Expressions;
using Core.Domain.Filtering;

namespace Core.Domain.Specifications;

public abstract class BaseFilteredSpecification<T> : BaseSpecification<T>
    where T : class
{
    protected BaseFilteredSpecification()
        : base(x => true)
    {
    }

    protected BaseFilteredSpecification(Expression<Func<T, bool>> filter)
        : base(filter)
    {
    }

    protected BaseFilteredSpecification(
        string? filterString,
        IFilterParser<T> filterParser)
        : base(filterString != null
            ? filterParser.Parse(filterString)
            : x => true)
    {
    }

    protected BaseFilteredSpecification(
        string? filterString,
        IFilterParser<T> filterParser,
        int pageNumber,
        int pageSize)
        : base(filterString != null
            ? filterParser.Parse(filterString)
            : x => true)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    protected BaseFilteredSpecification(
        string? filterString,
        IFilterParser<T>? filterParser,
        IFilterWhitelist? whitelist,
        int pageNumber,
        int pageSize)
        : base(ParseFilterWithWhitelist(filterString, filterParser, whitelist))
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    protected BaseFilteredSpecification(
        Expression<Func<T, bool>> filter,
        int pageNumber,
        int pageSize)
        : base(filter)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    private static Expression<Func<T, bool>> ParseFilterWithWhitelist(
        string? filterString,
        IFilterParser<T>? filterParser,
        IFilterWhitelist? whitelist)
    {
        if (string.IsNullOrWhiteSpace(filterString))
            return x => true;
        filterParser ??= new FilterParser<T>(null, whitelist);
        return filterParser.Parse(filterString);
    }

    protected void ApplySoftDeleteFilter()
    {
        if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var isDeletedProp = Expression.Property(parameter, "IsDeleted");
            var notDeleted = Expression.Not(isDeletedProp);
            var lambda = Expression.Lambda<Func<T, bool>>(notDeleted, parameter);
            AddCriteria(lambda);
        }
    }

    protected void ApplyOrderBy<TKey>(
        Expression<Func<T, TKey>> orderByExpression,
        bool orderByDescending = false)
    {
        if (orderByDescending)
            ApplyOrderByDescending(orderByExpression);
        else
            ApplyOrderBy(orderByExpression);
    }

    protected void AddIncludes(params Expression<Func<T, object?>>[] includes)
    {
        foreach (var include in includes) AddInclude(include);
    }
}