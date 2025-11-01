using System.Linq.Expressions;
namespace Core.Domain.Filtering;
public interface IFilterExpressionBuilder<T> where T : class
{
    Expression<Func<T, bool>> Build(FilterExpression filter);
}