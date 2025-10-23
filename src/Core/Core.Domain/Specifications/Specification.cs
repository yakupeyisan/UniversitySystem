namespace Core.Domain.Specifications;

using System.Linq.Expressions;

/// <summary>
/// Specification Pattern - Complex query logic encapsulation
/// 
/// Avantajları:
/// - Query logic'i reusable classes'ta encapsulate eder
/// - Repository'leri daha clean tutar
/// - Testing kolaylaşır
/// - Complex filtering logic'i organize eder
/// 
/// Kullanım:
/// var spec = new ActivePersonsSpecification(departmentId);
/// var persons = await _repository.GetAsync(spec);
/// </summary>
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

    /// <summary>
    /// Include - Navigation property eager load
    /// </summary>
    protected virtual void AddInclude(Expression<Func<TAggregate, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    /// <summary>
    /// Include string-based (for complex includes)
    /// </summary>
    protected virtual void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    /// <summary>
    /// Order by ascending
    /// </summary>
    protected virtual void AddOrderBy<TKey>(Expression<Func<TAggregate, TKey>> orderByExpression)
    {
        if (orderByExpression == null)
            throw new ArgumentNullException(nameof(orderByExpression));

        OrderBys.Add((CastExpression(orderByExpression), false));
    }

    /// <summary>
    /// Order by descending
    /// </summary>
    protected virtual void AddOrderByDescending<TKey>(Expression<Func<TAggregate, TKey>> orderByDescendingExpression)
    {
        if (orderByDescendingExpression == null)
            throw new ArgumentNullException(nameof(orderByDescendingExpression));

        OrderBys.Add((CastExpression(orderByDescendingExpression), true));
    }

    /// <summary>
    /// Pagination enable
    /// </summary>
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

    /// <summary>
    /// Paging'i deactivate et
    /// </summary>
    public virtual void RemovePaging()
    {
        Skip = null;
        Take = null;
        IsPagingEnabled = false;
    }

    /// <summary>
    /// Split Query enable et (EF Core 5+)
    /// 
    /// UseCase:
    /// - Multiple Include'larda SELECT N+1 yerine Multiple SELECT's yapılır
    /// - Bazı durumlarda performans improvement
    /// 
    /// Dikkat: JOIN'ler yerine separate SELECT'ler oluşur
    /// </summary>
    public virtual void UseSplitQuery()
    {
        IsSplitQuery = true;
    }

    /// <summary>
    /// Helper: Generic OrderBy expression'ı object'e cast et
    /// </summary>
    private static Expression<Func<TAggregate, object>> CastExpression<TKey>(Expression<Func<TAggregate, TKey>> source)
    {
        var parameter = source.Parameters[0];
        var body = Expression.Convert(source.Body, typeof(object));
        return Expression.Lambda<Func<TAggregate, object>>(body, parameter);
    }
}