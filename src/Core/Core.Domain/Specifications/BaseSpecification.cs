using System.Linq.Expressions;

namespace Core.Domain.Specifications;

/// <summary>
/// DDD Specification Pattern - Base class
/// 
/// Sorumluluğu:
/// - Query kriterlerini encapsulate etmek
/// - Include stratejisini yönetmek (eager loading)
/// - Paging ve ordering'i handle etmek
/// - Query'nin tüm detaylarını merkezi bir yerde toplamak
/// 
/// Avantajlar:
/// - Repository'ler basitleşir (GetAsync(spec))
/// - Query logic'i reusable hale gelir
/// - Testing kolaylaşır
/// - Domain logic'i organize edilir
/// 
/// Örnek:
/// public class GetActivePersonsSpec : BaseSpecification<Person>
/// {
///     public GetActivePersonsSpec()
///         : base(p => !p.IsDeleted)
///     {
///         AddInclude(p => p.Restrictions);
///         AddInclude(p => p.Student);
///         ApplyOrderBy(p => p.CreatedAt, orderByDescending: true);
///     }
/// }
/// 
/// Kullanım:
/// var spec = new GetActivePersonsSpec();
/// var result = await repository.GetAsync(spec);
/// </summary>
public abstract class BaseSpecification<T> : ISpecification<T> where T : class
{
    /// <summary>
    /// Query filter expression
    /// </summary>
    public Expression<Func<T, bool>>? Criteria { get; private set; }

    /// <summary>
    /// Include stratejileri (eager loading)
    /// Entity Framework: Include(p => p.Orders)
    /// </summary>
    public List<Expression<Func<T, object>>> Includes { get; } = new();

    /// <summary>
    /// String-based includes (navigation properties by name)
    /// Örnek: "Orders", "Address.Country"
    /// </summary>
    public List<string> IncludeStrings { get; } = new();

    /// <summary>
    /// OrderBy expressions (asc)
    /// </summary>
    public List<(Expression<Func<T, object>> OrderExpression, bool OrderByDescending)> OrderBys { get; } = new();

    /// <summary>
    /// Paging: Skip count
    /// </summary>
    public int? Take { get; private set; }

    /// <summary>
    /// Paging: Take count
    /// </summary>
    public int? Skip { get; private set; }

    /// <summary>
    /// Paging enabled?
    /// </summary>
    public bool IsPagingEnabled { get; private set; }

    /// <summary>
    /// Query result'ını tracked mi tracking disable edilecek
    /// Default: false (tracked)
    /// </summary>
    public bool IsSplitQuery { get; protected set; }

    // ============================================================================
    // Constructors
    // ============================================================================

    /// <summary>
    /// Empty specification - no criteria (returns all)
    /// </summary>
    protected BaseSpecification()
    {
        Criteria = null;
    }

    /// <summary>
    /// Specification with filter criteria
    /// 
    /// Örnek:
    /// base(p => p.IsActive && p.CreatedAt >= DateTime.UtcNow.AddDays(-30))
    /// </summary>
    protected BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria ?? throw new ArgumentNullException(nameof(criteria));
    }

    // ============================================================================
    // Protected Criteria Management
    // ============================================================================

    /// <summary>
    /// Criteria'yı güncelle (override)
    /// </summary>
    protected void AddCriteria(Expression<Func<T, bool>> criteria)
    {
        if (criteria == null)
            throw new ArgumentNullException(nameof(criteria));

        if (Criteria == null)
        {
            Criteria = criteria;
        }
        else
        {
            // ✅ Existing criteria ile AND'le
            var parameter = Expression.Parameter(typeof(T), "x");
            var left = Expression.Invoke(Criteria, parameter);
            var right = Expression.Invoke(criteria, parameter);
            var combined = Expression.AndAlso(left, right);
            Criteria = Expression.Lambda<Func<T, bool>>(combined, parameter);
        }
    }

    // ============================================================================
    // Include Management
    // ============================================================================

    /// <summary>
    /// Include ekleme (expression-based, type-safe)
    /// 
    /// Örnek:
    /// AddInclude(p => p.Orders)
    /// AddInclude(p => p.Address)
    /// 
    /// Nested: AddInclude(p => p.Orders.Select(o => o.Items))
    /// </summary>
    public virtual void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        if (includeExpression == null)
            throw new ArgumentNullException(nameof(includeExpression));

        Includes.Add(includeExpression);
    }

    /// <summary>
    /// String-based include (navigation property name)
    /// 
    /// Örnek:
    /// AddIncludeString("Orders")
    /// AddIncludeString("Address")
    /// AddIncludeString("Address.Country")  // nested
    /// </summary>
    public virtual void AddIncludeString(string navigationPropertyPath)
    {
        if (string.IsNullOrWhiteSpace(navigationPropertyPath))
            throw new ArgumentException("Navigation property path cannot be empty", nameof(navigationPropertyPath));

        IncludeStrings.Add(navigationPropertyPath);
    }

    // ============================================================================
    // Ordering Management
    // ============================================================================

    /// <summary>
    /// OrderBy (ascending)
    /// 
    /// Örnek:
    /// ApplyOrderBy(p => p.CreatedAt)
    /// </summary>
    protected virtual void ApplyOrderBy<TKey>(Expression<Func<T, TKey>> orderByExpression)
    {
        if (orderByExpression == null)
            throw new ArgumentNullException(nameof(orderByExpression));

        OrderBys.Add((CastExpression(orderByExpression), false));
    }

    /// <summary>
    /// OrderByDescending
    /// 
    /// Örnek:
    /// ApplyOrderByDescending(p => p.CreatedAt)
    /// </summary>
    protected virtual void ApplyOrderByDescending<TKey>(Expression<Func<T, TKey>> orderByExpression)
    {
        if (orderByExpression == null)
            throw new ArgumentNullException(nameof(orderByExpression));

        OrderBys.Add((CastExpression(orderByExpression), true));
    }

    /// <summary>
    /// Helper: Multiple OrderBy'lar eklemek için
    /// 
    /// Örnek:
    /// AddOrderBy(p => p.Priority, true);      // desc
    /// AddOrderBy(p => p.CreatedAt, false);    // asc
    /// </summary>
    protected void AddOrderBy<TKey>(Expression<Func<T, TKey>> orderByExpression, bool descending = false)
    {
        if (orderByExpression == null)
            throw new ArgumentNullException(nameof(orderByExpression));

        if (descending)
            ApplyOrderByDescending(orderByExpression);
        else
            ApplyOrderBy(orderByExpression);
    }

    // ============================================================================
    // Paging Management
    // ============================================================================

    /// <summary>
    /// Paging uyguła (Skip, Take)
    /// 
    /// Örnek:
    /// ApplyPaging(0, 20);    // Sayfa 1: record 0-19
    /// ApplyPaging(20, 20);   // Sayfa 2: record 20-39
    /// 
    /// Parameters:
    /// - skip: Kaçıncı recordtan başlanacak (0-based)
    /// - take: Kaç record alınacak
    /// </summary>
    public virtual void ApplyPaging(int skip, int take)
    {
        if (skip < 0)
            throw new ArgumentException("Skip value cannot be negative", nameof(skip));

        if (take <= 0)
            throw new ArgumentException("Take value must be greater than 0", nameof(take));

        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    /// <summary>
    /// Paging'i deactivate et
    /// </summary>
    public virtual void RemovePaging()
    {
        Skip = null;
        Take = null;
        IsPagingEnabled = false;
    }

    // ============================================================================
    // Query Strategy
    // ============================================================================

    /// <summary>
    /// Split Query enable et (EF Core 5+)
    /// 
    /// UseCase:
    /// - Multiple Include'larda SELECT N+1 yerine Multiple SELECT's yapılır
    /// - Bazı durumlarda performans improvement
    /// 
    /// Dikkat: JOIN'ler yerine separate SELECT'ler oluşur
    /// </summary>
    public virtual void UseSplitQuery()
    {
        IsSplitQuery = true;
    }

    // ============================================================================
    // Helper: Expression Casting
    // ============================================================================

    /// <summary>
    /// Generic OrderBy expression'ı object'e cast et
    /// Iç kullanım için
    /// </summary>
    private static Expression<Func<T, object>> CastExpression<TKey>(Expression<Func<T, TKey>> source)
    {
        var parameter = source.Parameters[0];
        var body = Expression.Convert(source.Body, typeof(object));
        return Expression.Lambda<Func<T, object>>(body, parameter);
    }
}