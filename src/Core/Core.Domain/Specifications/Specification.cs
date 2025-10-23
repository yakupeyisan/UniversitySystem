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
public abstract class Specification<TAggregate>: ISpecification<TAggregate>
    where TAggregate : AggregateRoot
{
    public Expression<Func<TAggregate, bool>>? Criteria { get; protected set; }
    public List<Expression<Func<TAggregate, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public Expression<Func<TAggregate, object>>? OrderBy { get; protected set; }
    public Expression<Func<TAggregate, object>>? OrderByDescending { get; protected set; }

    public int Take { get; protected set; }
    public int Skip { get; protected set; }
    public bool IsPagingEnabled { get; protected set; }

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
    protected virtual void AddOrderBy(Expression<Func<TAggregate, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    /// <summary>
    /// Order by descending
    /// </summary>
    protected virtual void AddOrderByDescending(Expression<Func<TAggregate, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }

    /// <summary>
    /// Pagination enable
    /// </summary>
    protected virtual void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }
}