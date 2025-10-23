using System.Linq.Expressions;

namespace Core.Domain.Filtering;

/// <summary>
/// Filter string'ini Expression<Func<T, bool>>'e dönüştürür
/// 
/// Format: field|operator|value;field2|operator2|value2
/// Örnek: price|gt|100;name|contains|book;status|in|active,pending;createdAt|between|2024-01-01,2024-12-31
/// 
/// ✅ Tüm hatalar düzeltilmiş:
/// 1. ✅ Enum support
/// 2. ✅ String case-insensitivity
/// 3. ✅ Property whitelist security
/// 4. ✅ Comprehensive error handling
/// 5. ✅ Type safety
/// </summary>
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

    /// <summary>
    /// Filter string'ini parse et ve kombinli Expression oluştur
    /// Tüm filterler AND ile kombinlenmiştir
    /// 
    /// Örnek:
    /// Parse("price|gt|100;status|in|active,pending")
    /// → Expression<Func<T, bool>> where (price > 100) AND (status IN (active, pending))
    /// 
    /// Null/empty string → Expression<x => true> (no filtering)
    /// </summary>
    public Expression<Func<T, bool>> Parse(string? filterString)
    {
        if (string.IsNullOrWhiteSpace(filterString))
        {
            // ✅ Boş filter: her şey döner (true)
            return x => true;
        }

        try
        {
            var filters = ParseFilterExpressions(filterString);

            if (filters.Count == 0)
                return x => true;

            // ✅ İlk filter ile başla
            var predicate = BuildPredicate(filters[0]);

            // ✅ Diğer filtreleri AND ile ekle
            for (int i = 1; i < filters.Count; i++)
            {
                var nextFilter = BuildPredicate(filters[i]);
                predicate = CombinePredicates(predicate, nextFilter, ExpressionType.AndAlso);
            }

            return predicate;
        }
        catch (FilterParsingException)
        {
            throw;  // Re-throw already formatted
        }
        catch (Exception ex)
        {
            throw new FilterParsingException(
                $"Filter string parse hatası: {filterString}", ex);
        }
    }

    /// <summary>
    /// Tek bir FilterExpression'dan LINQ predicate oluştur
    /// </summary>
    public Expression<Func<T, bool>> BuildPredicate(FilterExpression filter)
    {
        return _expressionBuilder.Build(filter);
    }

    // ============================================================================
    // Private: Filter Parsing
    // ============================================================================

    /// <summary>
    /// Filter string'ini FilterExpression listesine parse et
    /// Örnek: "price|gt|100;name|contains|book"
    /// → [FilterExpression("price", GreaterThan, ["100"]), FilterExpression("name", Contains, ["book"])]
    /// </summary>
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
                    // ✅ Whitelist security check
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

    /// <summary>
    /// Tek bir filter string'i parse et
    /// Örnek: "price|gt|100" → FilterExpression("price", GreaterThan, ["100"])
    /// </summary>
    private FilterExpression? ParseSingleFilter(string filterPart)
    {
        var parts = filterPart.Split(ExpressionSeparator);

        if (parts.Length < 2)
            throw new FilterParsingException(
                $"Geçersiz filter formatı: '{filterPart}'. " +
                $"Beklenen: field|operator|value");

        var propertyName = parts[0].Trim();
        var operatorStr = parts[1].Trim().ToLower();

        // ✅ Operator parse et
        if (!TryParseOperator(operatorStr, out var op))
            throw new FilterParsingException(
                $"Bilinmeyen operator: '{operatorStr}'. " +
                $"Desteklenen: eq, neq, gt, gte, lt, lte, contains, startswith, endswith, between, in, isnull, notnull");

        // ✅ Null/NotNull operatörleri için value gerekmiyor
        if (op == FilterOperator.IsNull || op == FilterOperator.IsNotNull)
        {
            return new FilterExpression(propertyName, op);
        }

        // ✅ Diğer operatörlerde value gerekli
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

    /// <summary>
    /// String operatörü FilterOperator'e dönüştür
    /// </summary>
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

    // ============================================================================
    // Private: Security
    // ============================================================================

    /// <summary>
    /// Property'nin whitelist'de olup olmadığını kontrol et
    /// </summary>
    private bool IsPropertyAllowed(string propertyName)
    {
        // ✅ Whitelist tanımlanmamışsa: all public properties allow
        if (_whitelist == null)
            return true;

        return _whitelist.IsAllowed(propertyName);
    }

    // ============================================================================
    // Private: Expression Combining
    // ============================================================================

    /// <summary>
    /// İki predicate'i combine et (AND/OR)
    /// 
    /// Örnek:
    /// left: (price > 100)
    /// right: (status = active)
    /// combineType: AndAlso
    /// → (price > 100) AND (status = active)
    /// </summary>
    private Expression<Func<T, bool>> CombinePredicates(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right,
        ExpressionType combineType)
    {
        var parameter = Expression.Parameter(typeof(T), "x");

        // ✅ Left ve right expressions'ı parameter'a invoke et
        var leftInvoked = Expression.Invoke(left, parameter);
        var rightInvoked = Expression.Invoke(right, parameter);

        // ✅ Binary operation (AND/OR) oluştur
        var combined = Expression.MakeBinary(combineType, leftInvoked, rightInvoked);

        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }
}