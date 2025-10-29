using System.Linq.Expressions;

namespace Core.Domain.Specifications;

public interface ISpecification<T> where T : class
{
    Expression<Func<T, bool>>? Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    List<string> IncludeStrings { get; }
    List<(Expression<Func<T, object>> OrderExpression, bool OrderByDescending)> OrderBys { get; }
    int? Take { get; }
    int? Skip { get; }
    bool IsPagingEnabled { get; }
    bool IsSplitQuery { get; }
}