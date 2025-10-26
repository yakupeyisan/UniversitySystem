namespace Core.Domain.Specifications;
using System.Linq.Expressions;
public abstract class Specification<TEntity> : ISpecification<TEntity>
    where TEntity : Entity
{
    public Expression<Func<TEntity, bool>>? Criteria { get; protected set; }
    public List<Expression<Func<TEntity, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public List<(Expression<Func<TEntity, object>> OrderExpression, bool OrderByDescending)> OrderBys { get; } = new();
    public int? Take { get; protected set; }
    public int? Skip { get; protected set; }
    public bool IsPagingEnabled { get; protected set; }
    public bool IsSplitQuery { get; protected set; }
    protected virtual void AddInclude(Expression<Func<TEntity, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }
    protected virtual void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }
    protected virtual void AddOrderBy<TKey>(Expression<Func<TEntity, TKey>> orderByExpression)
    {
        if (orderByExpression == null)
            throw new ArgumentNullException(nameof(orderByExpression));
        OrderBys.Add((CastExpression(orderByExpression), false));
    }
    protected virtual void AddOrderByDescending<TKey>(Expression<Func<TEntity, TKey>> orderByDescendingExpression)
    {
        if (orderByDescendingExpression == null)
            throw new ArgumentNullException(nameof(orderByDescendingExpression));
        OrderBys.Add((CastExpression(orderByDescendingExpression), true));
    }
    protected virtual void ApplyPaging(int skip, int take)
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
    private static Expression<Func<TEntity, object>> CastExpression<TKey>(Expression<Func<TEntity, TKey>> source)
    {
        var parameter = source.Parameters[0];
        var body = Expression.Convert(source.Body, typeof(object));
        return Expression.Lambda<Func<TEntity, object>>(body, parameter);
    }
}