using System.Linq.Expressions;

namespace Core.Domain.Filtering;

/// <summary>
/// FilterExpression'dan Lambda Expression oluşturan builder interface
/// 
/// Amaç:
/// - Type-safe LINQ expression generation
/// - LINQ to Entities / LINQ to Objects uyumlu
/// - Runtime reflection minimize etme
/// 
/// Sorumluluğu:
/// - Property name → Property expression mapping
/// - Type conversion (string → int, decimal, DateTime, Guid, Enum, etc.)
/// - Operator specific expression building (>,<,contains,between,in, etc.)
/// </summary>
public interface IFilterExpressionBuilder<T> where T : class
{
    /// <summary>
    /// FilterExpression'dan predicate oluştur
    /// 
    /// Örnek:
    /// Build(new FilterExpression("price", FilterOperator.GreaterThan, "100"))
    /// → Expression<Func<T, bool>> (x => x.Price > 100)
    /// 
    /// Exceptions:
    /// - FilterParsingException: Property bulunamazsa, type conversion başarısızsa
    /// </summary>
    Expression<Func<T, bool>> Build(FilterExpression filter);
}