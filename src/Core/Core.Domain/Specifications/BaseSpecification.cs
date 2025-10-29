using System.Linq.Expressions;

namespace Core.Domain.Specifications;

public abstract class BaseSpecification<T> : ISpecification<T> where T : class
{
    protected BaseSpecification()
    {
        Criteria = null;
    }

    protected BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria ?? throw new ArgumentNullException(nameof(criteria));
    }

    public Expression<Func<T, bool>>? Criteria { get; private set; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public List<(Expression<Func<T, object>> OrderExpression, bool OrderByDescending)> OrderBys { get; } = new();
    public int? Take { get; private set; }
    public int? Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }
    public bool IsSplitQuery { get; protected set; }

    protected void AddCriteria(Expression<Func<T, bool>> criteria)
    {
        if (criteria == null)
            throw new ArgumentNullException(nameof(criteria));
        if (Criteria == null)
        {
            Criteria = criteria;
        }
        else
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var left = Expression.Invoke(Criteria, parameter);
            var right = Expression.Invoke(criteria, parameter);
            var combined = Expression.AndAlso(left, right);
            Criteria = Expression.Lambda<Func<T, bool>>(combined, parameter);
        }
    }

    public virtual void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        if (includeExpression == null)
            throw new ArgumentNullException(nameof(includeExpression));
        Includes.Add(includeExpression);
    }

    public virtual void AddIncludeString(string navigationPropertyPath)
    {
        if (string.IsNullOrWhiteSpace(navigationPropertyPath))
            throw new ArgumentException("Navigation property path cannot be empty", nameof(navigationPropertyPath));
        IncludeStrings.Add(navigationPropertyPath);
    }

    protected virtual void ApplyOrderBy<TKey>(Expression<Func<T, TKey>> orderByExpression)
    {
        if (orderByExpression == null)
            throw new ArgumentNullException(nameof(orderByExpression));
        OrderBys.Add((CastExpression(orderByExpression), false));
    }

    protected virtual void ApplyOrderByDescending<TKey>(Expression<Func<T, TKey>> orderByExpression)
    {
        if (orderByExpression == null)
            throw new ArgumentNullException(nameof(orderByExpression));
        OrderBys.Add((CastExpression(orderByExpression), true));
    }

    protected void AddOrderBy<TKey>(Expression<Func<T, TKey>> orderByExpression, bool descending = false)
    {
        if (orderByExpression == null)
            throw new ArgumentNullException(nameof(orderByExpression));
        if (descending)
            ApplyOrderByDescending(orderByExpression);
        else
            ApplyOrderBy(orderByExpression);
    }

    public virtual void ApplyPaging(int skip, int take)
    {
        if (skip < 0)
            throw new ArgumentException("Skip value cannot be negative", nameof(skip));
        if (take <= 0)
            throw new ArgumentException("Take value must be greater than 0", nameof(take));
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    public virtual void RemovePaging()
    {
        Skip = null;
        Take = null;
        IsPagingEnabled = false;
    }

    public virtual void UseSplitQuery()
    {
        IsSplitQuery = true;
    }

    private static Expression<Func<T, object>> CastExpression<TKey>(Expression<Func<T, TKey>> source)
    {
        var parameter = source.Parameters[0];
        var body = Expression.Convert(source.Body, typeof(object));
        return Expression.Lambda<Func<T, object>>(body, parameter);
    }
}