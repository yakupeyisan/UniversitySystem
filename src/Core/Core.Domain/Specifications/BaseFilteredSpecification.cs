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