using System.Linq.Expressions;

namespace Core.Domain.Filtering;

public interface IFilterParser<T> where T : class
{
    Expression<Func<T, bool>> Parse(string? filterString);
    Expression<Func<T, bool>> BuildPredicate(FilterExpression filter);
}