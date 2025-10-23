using System.Linq.Expressions;

namespace Core.Domain.Filtering;

/// <summary>
/// Filter string'ini Expression<Func<T, bool>>'e dönüştüren interface
/// 
/// Kullanım:
/// "price|gt|100;name|contains|book" → Expression<Func<Product, bool>>
/// 
/// Operatörler:
/// - eq, neq → equals, not equals
/// - gt, gte, lt, lte → comparison
/// - contains, startswith, endswith → string operations
/// - between → range (2 values)
/// - in → multiple values (OR)
/// - isnull, notnull → null check
/// 
/// Format:
/// field|operator|value;field2|operator2|value2
/// Tüm filterler AND'lenmiştir
/// </summary>
public interface IFilterParser<T> where T : class
{
    /// <summary>
    /// Filter string'ini parse edip Expression oluştur
    /// 
    /// Örnek:
    /// Parse("age|gt|18;status|in|active,pending")
    /// → (age > 18) AND (status IN (active, pending))
    /// 
    /// Null/empty string → Expression<x => true> (no filtering)
    /// </summary>
    Expression<Func<T, bool>> Parse(string? filterString);

    /// <summary>
    /// Tek bir FilterExpression'dan LINQ predicate oluştur
    /// 
    /// Örnek:
    /// BuildPredicate(new FilterExpression("price", FilterOperator.GreaterThan, "100"))
    /// → Expression<x => x.Price > 100>
    /// </summary>
    Expression<Func<T, bool>> BuildPredicate(FilterExpression filter);
}