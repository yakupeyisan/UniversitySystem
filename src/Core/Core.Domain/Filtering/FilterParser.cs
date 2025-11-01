using System.Linq.Expressions;
namespace Core.Domain.Filtering;
public class FilterParser<T> : IFilterParser<T> where T : class
{
    private const char FilterSeparator = ';';
    private const char ExpressionSeparator = '|';
    private const char ValueSeparator = ',';
    private readonly IFilterExpressionBuilder<T> _expressionBuilder;
    private readonly IFilterWhitelist? _whitelist;
    public FilterParser(
        IFilterExpressionBuilder<T>? expressionBuilder = null,
        IFilterWhitelist? whitelist = null)
    {
        _expressionBuilder = expressionBuilder ?? new FilterExpressionBuilder<T>();
        _whitelist = whitelist;
    }
    public Expression<Func<T, bool>> Parse(string? filterString)
    {
        if (string.IsNullOrWhiteSpace(filterString)) return x => true;
        try
        {
            var filters = ParseFilterExpressions(filterString);
            if (filters.Count == 0)
                return x => true;
            var predicate = BuildPredicate(filters[0]);
            for (var i = 1; i < filters.Count; i++)
            {
                var nextFilter = BuildPredicate(filters[i]);
                predicate = CombinePredicates(predicate, nextFilter, ExpressionType.AndAlso);
            }
            return predicate;
        }
        catch (FilterParsingException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new FilterParsingException(
                $"Filter string parse hatası: {filterString}", ex);
        }
    }
    public Expression<Func<T, bool>> BuildPredicate(FilterExpression filter)
    {
        return _expressionBuilder.Build(filter);
    }
    private List<FilterExpression> ParseFilterExpressions(string filterString)
    {
        var result = new List<FilterExpression>();
        var filterParts = filterString.Split(FilterSeparator);
        foreach (var part in filterParts)
        {
            if (string.IsNullOrWhiteSpace(part))
                continue;
            try
            {
                var filter = ParseSingleFilter(part.Trim());
                if (filter != null)
                {
                    if (!IsPropertyAllowed(filter.PropertyName))
                        throw new FilterParsingException(
                            $"Property '{filter.PropertyName}' filter'da kullanılamaz (whitelist tarafından block edildi)");
                    result.Add(filter);
                }
            }
            catch (FilterParsingException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FilterParsingException(
                    $"Single filter parse hatası: '{part}'", ex);
            }
        }
        return result;
    }
    private FilterExpression? ParseSingleFilter(string filterPart)
    {
        var parts = filterPart.Split(ExpressionSeparator);
        if (parts.Length < 2)
            throw new FilterParsingException(
                $"Geçersiz filter formatı: '{filterPart}'. " +
                $"Beklenen: field|operator|value");
        var propertyName = parts[0].Trim();
        var operatorStr = parts[1].Trim().ToLower();
        if (!TryParseOperator(operatorStr, out var op))
            throw new FilterParsingException(
                $"Bilinmeyen operator: '{operatorStr}'. " +
                $"Desteklenen: eq, neq, gt, gte, lt, lte, contains, startswith, endswith, between, in, isnull, notnull");
        if (op == FilterOperator.IsNull || op == FilterOperator.IsNotNull)
            return new FilterExpression(propertyName, op);
        if (parts.Length < 3)
            throw new FilterParsingException(
                $"Operator '{operatorStr}' için değer gerekli. " +
                $"Format: field|{operatorStr}|value");
        var valueStr = string.Join(ExpressionSeparator, parts.Skip(2)).Trim();
        var values = valueStr.Split(ValueSeparator)
            .Select(v => v.Trim())
            .Where(v => !string.IsNullOrEmpty(v))
            .ToList();
        if (values.Count == 0)
            throw new FilterParsingException(
                $"Operator '{operatorStr}' için en az 1 değer gerekli");
        return new FilterExpression(propertyName, op, values.ToArray());
    }
    private bool TryParseOperator(string operatorStr, out FilterOperator result)
    {
        result = operatorStr switch
        {
            "eq" => FilterOperator.Equals,
            "neq" => FilterOperator.NotEquals,
            "gt" => FilterOperator.GreaterThan,
            "gte" => FilterOperator.GreaterOrEqual,
            "lt" => FilterOperator.LessThan,
            "lte" => FilterOperator.LessOrEqual,
            "contains" => FilterOperator.Contains,
            "startswith" => FilterOperator.StartsWith,
            "endswith" => FilterOperator.EndsWith,
            "between" => FilterOperator.Between,
            "in" => FilterOperator.In,
            "isnull" => FilterOperator.IsNull,
            "notnull" => FilterOperator.IsNotNull,
            _ => FilterOperator.Equals
        };
        return true;
    }
    private bool IsPropertyAllowed(string propertyName)
    {
        if (_whitelist == null)
            return true;
        return _whitelist.IsAllowed(propertyName);
    }
    private Expression<Func<T, bool>> CombinePredicates(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right,
        ExpressionType combineType)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var leftInvoked = Expression.Invoke(left, parameter);
        var rightInvoked = Expression.Invoke(right, parameter);
        var combined = Expression.MakeBinary(combineType, leftInvoked, rightInvoked);
        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }
}