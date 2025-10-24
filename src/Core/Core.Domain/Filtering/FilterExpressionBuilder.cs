using System.Globalization;
using System.Linq.Expressions;
namespace Core.Domain.Filtering;
public class FilterExpressionBuilder<T> : IFilterExpressionBuilder<T> where T : class
{
    public Expression<Func<T, bool>> Build(FilterExpression filter)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = GetPropertyExpression(parameter, filter.PropertyName);
        if (property == null)
            throw new FilterParsingException(
                $"Property '{filter.PropertyName}' bulunamadı veya accessible değil",
                filter.PropertyName);
        var predicate = BuildFilterExpression(property, filter);
        return Expression.Lambda<Func<T, bool>>(predicate, parameter);
    }
    private Expression BuildFilterExpression(Expression property, FilterExpression filter)
    {
        return filter.Operator switch
        {
            FilterOperator.Equals => BuildEquals(property, filter.Values[0]),
            FilterOperator.NotEquals => BuildNotEquals(property, filter.Values[0]),
            FilterOperator.GreaterThan => BuildGreaterThan(property, filter.Values[0]),
            FilterOperator.GreaterOrEqual => BuildGreaterOrEqual(property, filter.Values[0]),
            FilterOperator.LessThan => BuildLessThan(property, filter.Values[0]),
            FilterOperator.LessOrEqual => BuildLessOrEqual(property, filter.Values[0]),
            FilterOperator.Contains => BuildContains(property, filter.Values[0]),
            FilterOperator.StartsWith => BuildStartsWith(property, filter.Values[0]),
            FilterOperator.EndsWith => BuildEndsWith(property, filter.Values[0]),
            FilterOperator.Between => BuildBetween(property, filter.Values),
            FilterOperator.In => BuildIn(property, filter.Values),
            FilterOperator.IsNull => BuildIsNull(property),
            FilterOperator.IsNotNull => BuildIsNotNull(property),
            _ => throw new FilterParsingException($"Bilinmeyen operator: {filter.Operator}")
        };
    }
    private Expression? GetPropertyExpression(ParameterExpression parameter, string propertyPath)
    {
        if (string.IsNullOrWhiteSpace(propertyPath))
            return null;
        var properties = propertyPath.Split('.');
        Expression? expression = parameter;
        foreach (var propName in properties)
        {
            if (expression == null)
                return null;
            var property = expression.Type.GetProperty(propName,
                System.Reflection.BindingFlags.IgnoreCase |
                System.Reflection.BindingFlags.Public);
            if (property == null)
                return null;
            expression = Expression.Property(expression, property);
        }
        return expression;
    }
    private Expression BuildEquals(Expression property, string value)
    {
        var constant = ConvertToConstant(property.Type, value);
        return Expression.Equal(property, constant);
    }
    private Expression BuildNotEquals(Expression property, string value)
    {
        var constant = ConvertToConstant(property.Type, value);
        return Expression.NotEqual(property, constant);
    }
    private Expression BuildGreaterThan(Expression property, string value)
    {
        var constant = ConvertToConstant(property.Type, value);
        return Expression.GreaterThan(property, constant);
    }
    private Expression BuildGreaterOrEqual(Expression property, string value)
    {
        var constant = ConvertToConstant(property.Type, value);
        return Expression.GreaterThanOrEqual(property, constant);
    }
    private Expression BuildLessThan(Expression property, string value)
    {
        var constant = ConvertToConstant(property.Type, value);
        return Expression.LessThan(property, constant);
    }
    private Expression BuildLessOrEqual(Expression property, string value)
    {
        var constant = ConvertToConstant(property.Type, value);
        return Expression.LessThanOrEqual(property, constant);
    }
    private Expression BuildContains(Expression property, string value)
    {
        if (property.Type != typeof(string))
            throw new FilterParsingException(
                $"Contains operatörü sadece string property'de kullanılabilir. " +
                $"Property tipi: {property.Type.Name}");
        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
        if (toLowerMethod == null)
            throw new FilterParsingException("String.ToLower() method bulunamadı");
        var toLowerCall = Expression.Call(property, toLowerMethod);
        var containsMethod = typeof(string).GetMethod("Contains",
            new[] { typeof(string) });
        if (containsMethod == null)
            throw new FilterParsingException("String.Contains() method bulunamadı");
        return Expression.Call(toLowerCall, containsMethod,
            Expression.Constant(value.ToLower()));
    }
    private Expression BuildStartsWith(Expression property, string value)
    {
        if (property.Type != typeof(string))
            throw new FilterParsingException(
                $"StartsWith operatörü sadece string property'de kullanılabilir");
        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
        if (toLowerMethod == null)
            throw new FilterParsingException("String.ToLower() method bulunamadı");
        var toLowerCall = Expression.Call(property, toLowerMethod);
        var startsWithMethod = typeof(string).GetMethod("StartsWith",
            new[] { typeof(string) });
        if (startsWithMethod == null)
            throw new FilterParsingException("String.StartsWith() method bulunamadı");
        return Expression.Call(toLowerCall, startsWithMethod,
            Expression.Constant(value.ToLower()));
    }
    private Expression BuildEndsWith(Expression property, string value)
    {
        if (property.Type != typeof(string))
            throw new FilterParsingException(
                $"EndsWith operatörü sadece string property'de kullanılabilir");
        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
        if (toLowerMethod == null)
            throw new FilterParsingException("String.ToLower() method bulunamadı");
        var toLowerCall = Expression.Call(property, toLowerMethod);
        var endsWithMethod = typeof(string).GetMethod("EndsWith",
            new[] { typeof(string) });
        if (endsWithMethod == null)
            throw new FilterParsingException("String.EndsWith() method bulunamadı");
        return Expression.Call(toLowerCall, endsWithMethod,
            Expression.Constant(value.ToLower()));
    }
    private Expression BuildBetween(Expression property, List<string> values)
    {
        if (values.Count < 2)
            throw new FilterParsingException(
                "Between operatörü için 2 değer gerekli (min,max)");
        var value1 = ConvertToConstant(property.Type, values[0]);
        var value2 = ConvertToConstant(property.Type, values[1]);
        var greaterOrEqual = Expression.GreaterThanOrEqual(property, value1);
        var lessOrEqual = Expression.LessThanOrEqual(property, value2);
        return Expression.AndAlso(greaterOrEqual, lessOrEqual);
    }
    private Expression BuildIn(Expression property, List<string> values)
    {
        if (values.Count == 0)
            throw new FilterParsingException(
                "In operatörü için en az 1 değer gerekli");
        Expression? expression = null;
        foreach (var value in values)
        {
            var constant = ConvertToConstant(property.Type, value);
            var equal = Expression.Equal(property, constant);
            expression = expression == null
                ? equal
                : Expression.OrElse(expression, equal);
        }
        return expression ?? Expression.Constant(false);
    }
    private Expression BuildIsNull(Expression property)
    {
        return Expression.Equal(property, Expression.Constant(null));
    }
    private Expression BuildIsNotNull(Expression property)
    {
        return Expression.NotEqual(property, Expression.Constant(null));
    }
    private Expression ConvertToConstant(Type targetType, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new FilterParsingException($"Değer boş olamaz");
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;
        try
        {
            object? convertedValue = underlyingType switch
            {
                _ when underlyingType == typeof(string) => value,
                _ when underlyingType == typeof(int) => int.Parse(value),
                _ when underlyingType == typeof(long) => long.Parse(value),
                _ when underlyingType == typeof(decimal) =>
                    decimal.Parse(value, CultureInfo.InvariantCulture),
                _ when underlyingType == typeof(double) =>
                    double.Parse(value, CultureInfo.InvariantCulture),
                _ when underlyingType == typeof(float) =>
                    float.Parse(value, CultureInfo.InvariantCulture),
                _ when underlyingType == typeof(bool) =>
                    bool.Parse(value),
                _ when underlyingType == typeof(DateTime) =>
                    DateTime.Parse(value, CultureInfo.InvariantCulture),
                _ when underlyingType == typeof(DateOnly) =>
                    DateOnly.Parse(value),
                _ when underlyingType == typeof(Guid) =>
                    Guid.Parse(value),
                _ when underlyingType.IsEnum =>
                    Enum.Parse(underlyingType, value, ignoreCase: true),
                _ => throw new FilterParsingException(
                    $"Tip dönüşümü desteklenmiyor: {underlyingType.Name}")
            };
            return Expression.Constant(convertedValue, targetType);
        }
        catch (FilterParsingException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new FilterParsingException(
                $"'{value}' değeri {underlyingType.Name} tipine dönüştürülemedi: {ex.Message}",
                ex);
        }
    }
}