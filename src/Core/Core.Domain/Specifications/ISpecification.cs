using System.Linq.Expressions;

namespace Core.Domain.Specifications;

/// <summary>
/// ISpecification - Specification Pattern Interface
/// 
/// Specification Pattern:
/// - Complex query logic'i encapsulate eder
/// - Reusable ve testable
/// - Repository'leri clean tutar
/// - Business logic'i domain'de tutar
/// 
/// İçeriği:
/// ✅ Criteria (WHERE clause)
/// ✅ Includes (Navigation properties)
/// ✅ OrderBy (Sorting)
/// ✅ Paging (Skip & Take - optional)
/// </summary>
public interface ISpecification<TAggregate> where TAggregate : AggregateRoot
{
    /// <summary>
    /// WHERE clause - Filter condition
    /// Örnek: p => !p.IsDeleted && p.DepartmentId == deptId
    /// </summary>
    Expression<Func<TAggregate, bool>>? Criteria { get; }

    /// <summary>
    /// Navigation properties eager load (LINQ expression)
    /// Örnek: p => p.Student, p => p.HealthRecord
    /// </summary>
    List<Expression<Func<TAggregate, object>>> Includes { get; }

    /// <summary>
    /// Navigation properties eager load (string-based)
    /// Örnek: "Student", "HealthRecord", "Restrictions"
    /// Complex includes için: "Student.Enrollments.Courses"
    /// </summary>
    List<string> IncludeStrings { get; }

    /// <summary>
    /// ORDER BY expression (ascending)
    /// </summary>
    Expression<Func<TAggregate, object>>? OrderBy { get; }

    /// <summary>
    /// ORDER BY DESC expression
    /// </summary>
    Expression<Func<TAggregate, object>>? OrderByDescending { get; }

    /// <summary>
    /// SKIP count (pagination)
    /// Örnek: Page 1 = 0, Page 2 = 20, Page 3 = 40...
    /// </summary>
    int Skip { get; }

    /// <summary>
    /// TAKE count (page size)
    /// </summary>
    int Take { get; }

    /// <summary>
    /// Pagination enabled mi?
    /// </summary>
    bool IsPagingEnabled { get; }
}