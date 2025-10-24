namespace Core.Domain.Specifications;
using System.Linq.Expressions;
public abstract class Specification<TAggregate> : ISpecification<TAggregate>
    where TAggregate : AggregateRoot
{
    public Expression<Func<TAggregate, bool>>? Criteria { get; protected set; }
    public List<Expression<Func<TAggregate, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public List<(Expression<Func<TAggregate, object>> OrderExpression, bool OrderByDescending)> OrderBys { get; } = new();
    public int? Take { get; protected set; }
    public int? Skip { get; protected set; }
    public bool IsPagingEnabled { get; protected set; }
    public bool IsSplitQuery { get; protected set; }
    protected virtual void AddInclude(Expression<Func<TAggregate, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }
    protected virtual void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }
    protected virtual void AddOrderBy<TKey>(Expression<Func<TAggregate, TKey>> orderByExpression)
    {
        if (orderByExpression == null)
            throw new ArgumentNullException(nameof(orderByExpression));
        OrderBys.Add((CastExpression(orderByExpression), false));
    }
    protected virtual void AddOrderByDescending<TKey>(Expression<Func<TAggregate, TKey>> orderByDescendingExpression)
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
    private static Expression<Func<TAggregate, object>> CastExpression<TKey>(Expression<Func<TAggregate, TKey>> source)
    {
        var parameter = source.Parameters[0];
        var body = Expression.Convert(source.Body, typeof(object));
        return Expression.Lambda<Func<TAggregate, object>>(body, parameter);
    }
}