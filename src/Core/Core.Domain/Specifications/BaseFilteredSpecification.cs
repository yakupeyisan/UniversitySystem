using System.Linq.Expressions;
using Core.Domain.Filtering;

namespace Core.Domain.Specifications;


/// <summary>
/// Dynamic filtering desteği olan base Specification
/// 
/// DDD Specification Pattern'ı extend etmiştir:
/// - BaseSpecification<T>: Temel query logic
/// - BaseFilteredSpecification<T>: + Dinamik filter support
/// 
/// Amaç:
/// - Repository pattern'ı simplify etmek
/// - Query logic'i centralize etmek
/// - Paging + Filtering beraber yönetmek
/// - Include strategies'i organize etmek
/// 
/// Kullanım:
/// public class GetPersonsWithFilterSpec : BaseFilteredSpecification<Person>
/// {
///     public GetPersonsWithFilterSpec(string? filterString, int page = 1, int pageSize = 10)
///         : base(filterString, new FilterParser<Person>(), page, pageSize)
///     {
///         AddInclude(p => p.Restrictions);
///         ApplyOrderBy(p => p.CreatedAt, orderByDescending: true);
///     }
/// }
/// 
/// var spec = new GetPersonsWithFilterSpec("firstName|contains|John;createdAt|gte|2024-01-01");
/// var result = await _repository.GetAsync(spec);
/// </summary>
public abstract class BaseFilteredSpecification<T> : BaseSpecification<T>
    where T : class
{
    /// <summary>
    /// Empty filter - hiç condition yok
    /// </summary>
    protected BaseFilteredSpecification()
        : base(x => true)
    {
    }

    /// <summary>
    /// Manual filter expression ile
    /// </summary>
    protected BaseFilteredSpecification(Expression<Func<T, bool>> filter)
        : base(filter)
    {
    }

    /// <summary>
    /// Filter string ile (IFilterParser required)
    /// 
    /// Örnek:
    /// base("firstName|contains|John;gender|eq|Male", new FilterParser<Person>())
    /// 
    /// ✅ Specification criteria'ya parse edilen filter'ı ekler
    /// </summary>
    protected BaseFilteredSpecification(
        string? filterString,
        IFilterParser<T> filterParser)
        : base(filterString != null
            ? filterParser.Parse(filterString)
            : x => true)
    {
    }

    /// <summary>
    /// Filter string + Paging ile
    /// 
    /// Örnek:
    /// base("firstName|contains|John", new FilterParser<Person>(), pageNumber: 1, pageSize: 20)
    /// → Sayfa 1, 20 öğe, filter uygulanmış
    /// </summary>
    protected BaseFilteredSpecification(
        string? filterString,
        IFilterParser<T> filterParser,
        int pageNumber,
        int pageSize)
        : base(filterString != null
            ? filterParser.Parse(filterString)
            : x => true)
    {
        // ✅ Paging uygulandı (0-based offset)
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    /// <summary>
    /// Filter string + IFilterWhitelist + Paging ile
    /// 
    /// ✅ Advanced: Whitelist security ile
    /// </summary>
    protected BaseFilteredSpecification(
        string? filterString,
        IFilterParser<T>? filterParser,
        IFilterWhitelist? whitelist,
        int pageNumber,
        int pageSize)
        : base(ParseFilterWithWhitelist(filterString, filterParser, whitelist))
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    /// <summary>
    /// Manual filter + Paging ile
    /// </summary>
    protected BaseFilteredSpecification(
        Expression<Func<T, bool>> filter,
        int pageNumber,
        int pageSize)
        : base(filter)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    // ============================================================================
    // Protected Helpers
    // ============================================================================

    /// <summary>
    /// Filter string'i whitelist ile parse et
    /// </summary>
    private static Expression<Func<T, bool>> ParseFilterWithWhitelist(
        string? filterString,
        IFilterParser<T>? filterParser,
        IFilterWhitelist? whitelist)
    {
        if (string.IsNullOrWhiteSpace(filterString))
            return x => true;

        filterParser ??= new FilterParser<T>(null, whitelist);
        return filterParser.Parse(filterString);
    }

    /// <summary>
    /// Soft delete filter'ı ekle (ISoftDelete entities için)
    /// 
    /// ⚠️ IMPORTANT: Specification'da her zaman çağrılmalı!
    /// Örnek:
    /// protected GetActivePersonsSpec()
    /// {
    ///     ApplySoftDeleteFilter();  // ✅ MUST HAVE
    /// }
    /// </summary>
    protected void ApplySoftDeleteFilter()
    {
        // ✅ Type check: ISoftDelete implement ediyor mu?
        if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
        {
            // ✅ Dynamic predicate: x => !x.IsDeleted
            var parameter = Expression.Parameter(typeof(T), "x");
            var isDeletedProp = Expression.Property(parameter, "IsDeleted");
            var notDeleted = Expression.Not(isDeletedProp);
            var lambda = Expression.Lambda<Func<T, bool>>(notDeleted, parameter);

            AddCriteria(lambda);
        }
    }

    /// <summary>
    /// Ordering ekle (descending option'ı ile)
    /// </summary>
    protected void ApplyOrderBy<TKey>(
        Expression<Func<T, TKey>> orderByExpression,
        bool orderByDescending = false)
    {
        if (orderByDescending)
        {
            ApplyOrderByDescending(orderByExpression);
        }
        else
        {
            ApplyOrderBy(orderByExpression);
        }
    }

    /// <summary>
    /// Multiple include'ları kolayca ekle
    /// 
    /// Örnek:
    /// AddIncludes(
    ///     p => p.Student,
    ///     p => p.Staff,
    ///     p => p.HealthRecord,
    ///     p => p.Restrictions
    /// );
    /// </summary>
    protected void AddIncludes(params Expression<Func<T, object?>>[] includes)
    {
        foreach (var include in includes)
        {
            AddInclude(include);
        }
    }
}

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

        OrderBys.Add((orderByExpression.Cast(), false));
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

        OrderBys.Add((orderByExpression.Cast(), true));
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
    private static Expression<Func<T, object>> Cast<TKey>(Expression<Func<T, TKey>> source)
    {
        var parameter = source.Parameters[0];
        var body = Expression.Convert(source.Body, typeof(object));
        return Expression.Lambda<Func<T, object>>(body, parameter);
    }
}
