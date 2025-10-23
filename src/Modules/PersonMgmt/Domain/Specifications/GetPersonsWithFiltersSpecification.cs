using System.Linq.Expressions;
using Core.Domain.Filtering;
using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Filtering;

namespace PersonMgmt.Domain.Specifications;

/// <summary>
/// Specification - Dinamik filtering + Paging ile kişileri getir
/// 
/// BaseFilteredSpecification'ı extend ederek:
/// - Dynamic filter string support
/// - Pagination support
/// - Security whitelist ile property filtering
/// 
/// Kullanım:
/// 
/// Örnek 1: Tüm aktif kişiler (filter yok)
/// var spec = new GetPersonsWithFiltersSpecification();
/// var persons = await _repository.GetAsync(spec);
/// 
/// Örnek 2: Email'e göre filtrele
/// var spec = new GetPersonsWithFiltersSpecification("email|contains|@university.edu");
/// var persons = await _repository.GetAsync(spec);
/// 
/// Örnek 3: Multiple filters + paging
/// var spec = new GetPersonsWithFiltersSpecification(
///     "email|contains|@university.edu;gender|eq|Male",
///     pageNumber: 1,
///     pageSize: 20);
/// var persons = await _repository.GetAsync(spec);
/// 
/// Filter Format: field|operator|value;field2|operator2|value2
/// 
/// Desteklenen operatörler:
/// - eq → Eşittir
/// - neq → Eşit değildir
/// - contains → İçerir
/// - startswith → Başlangıç
/// - endswith → Son
/// - gt, gte, lt, lte → Comparison
/// - between → Aralık (value1,value2)
/// - in → Listede (value1,value2,value3)
/// 
/// Örnekler:
/// "email|contains|john@"
/// "gender|eq|Male;birthDate|gte|1990-01-01"
/// "name|contains|John;phoneNumber|startswith|+90"
/// "departmentId|eq|550e8400-e29b-41d4-a716-446655440000"
/// </summary>
public class GetPersonsWithFiltersSpecification : BaseFilteredSpecification<Person>
{
    /// <summary>
    /// Tüm aktif kişiler (filter yok)
    /// </summary>
    public GetPersonsWithFiltersSpecification()
        : base(x => !x.IsDeleted)
    {
        AddIncludes(
            p => p.Student,
            p => p.Staff,
            p => p.HealthRecord,
            p => p.Restrictions
        );
        ApplyOrderBy(p => p.Name);
    }

    /// <summary>
    /// Dinamik filter ile kişiler
    /// 
    /// Örnek:
    /// "email|contains|@example.com"
    /// "gender|eq|Male;birthDate|gte|1990-01-01"
    /// </summary>
    public GetPersonsWithFiltersSpecification(string? filterString)
        : base(filterString, new FilterParser<Person>(null, new PersonFilterWhitelist()))
    {
        // ✅ Base criteria'ya soft delete check eklendi
        AddCriteria(x => !x.IsDeleted);

        // ✅ Include strategies
        AddIncludes(
            p => p.Student,
            p => p.Staff,
            p => p.HealthRecord,
            p => p.Restrictions
        );

        // ✅ Default ordering
        ApplyOrderBy(p => p.Name);
    }

    /// <summary>
    /// Dinamik filter + Paging ile kişiler
    /// 
    /// Örnek:
    /// new GetPersonsWithFiltersSpecification(
    ///     "email|contains|@university.edu",
    ///     pageNumber: 1,
    ///     pageSize: 20)
    /// </summary>
    public GetPersonsWithFiltersSpecification(
        string? filterString,
        int pageNumber,
        int pageSize)
        : base(
            filterString,
            new FilterParser<Person>(null, new PersonFilterWhitelist()),
            pageNumber,
            pageSize)
    {
        // ✅ Base criteria'ya soft delete check eklendi
        AddCriteria(x => !x.IsDeleted);

        // ✅ Include strategies
        AddIncludes(
            p => p.Student,
            p => p.Staff,
            p => p.HealthRecord,
            p => p.Restrictions
        );

        // ✅ Default ordering
        ApplyOrderBy(p => p.Name);
    }

    /// <summary>
    /// Filter string + Custom ordering ile
    /// 
    /// Örnek:
    /// new GetPersonsWithFiltersSpecification(
    ///     "email|contains|@university.edu",
    ///     orderBy: p => p.CreatedAt,
    ///     descending: true)
    /// </summary>
    public GetPersonsWithFiltersSpecification(
        string? filterString,
        Expression<Func<Person, object>>? orderBy = null,
        bool descending = false)
        : base(filterString, new FilterParser<Person>(null, new PersonFilterWhitelist()))
    {
        // ✅ Base criteria'ya soft delete check eklendi
        AddCriteria(x => !x.IsDeleted);

        // ✅ Include strategies
        AddIncludes(
            p => p.Student,
            p => p.Staff,
            p => p.HealthRecord,
            p => p.Restrictions
        );

        // ✅ Custom ordering (eğer sağlanmışsa)
        if (orderBy != null)
        {
            if (descending)
            {
                OrderBys.Clear();
                OrderBys.Add((orderBy, true));
            }
            else
            {
                OrderBys.Clear();
                OrderBys.Add((orderBy, false));
            }
        }
        else
        {
            ApplyOrderBy(p => p.Name);
        }
    }

    /// <summary>
    /// Filter string + Paging + Custom ordering ile
    /// 
    /// Örnek:
    /// new GetPersonsWithFiltersSpecification(
    ///     "email|contains|@university.edu",
    ///     pageNumber: 1,
    ///     pageSize: 20,
    ///     orderBy: p => p.CreatedAt,
    ///     descending: true)
    /// </summary>
    public GetPersonsWithFiltersSpecification(
        string? filterString,
        int pageNumber,
        int pageSize,
        Expression<Func<Person, object>>? orderBy = null,
        bool descending = false)
        : base(
            filterString,
            new FilterParser<Person>(null, new PersonFilterWhitelist()),
            pageNumber,
            pageSize)
    {
        // ✅ Base criteria'ya soft delete check eklendi
        AddCriteria(x => !x.IsDeleted);

        // ✅ Include strategies
        AddIncludes(
            p => p.Student,
            p => p.Staff,
            p => p.HealthRecord,
            p => p.Restrictions
        );

        // ✅ Custom ordering (eğer sağlanmışsa)
        if (orderBy != null)
        {
            if (descending)
            {
                OrderBys.Clear();
                OrderBys.Add((orderBy, true));
            }
            else
            {
                OrderBys.Clear();
                OrderBys.Add((orderBy, false));
            }
        }
        else
        {
            ApplyOrderBy(p => p.Name);
        }
    }

    /// <summary>
    /// Filter string + Whitelist + Paging ile
    /// 
    /// Advanced: Custom whitelist sağlama
    /// </summary>
    public GetPersonsWithFiltersSpecification(
        string? filterString,
        IFilterParser<Person> customFilterParser,
        int pageNumber,
        int pageSize)
        : base(filterString, customFilterParser, pageNumber, pageSize)
    {
        // ✅ Base criteria'ya soft delete check eklendi
        AddCriteria(x => !x.IsDeleted);

        // ✅ Include strategies
        AddIncludes(
            p => p.Student,
            p => p.Staff,
            p => p.HealthRecord,
            p => p.Restrictions
        );

        // ✅ Default ordering
        ApplyOrderBy(p => p.Name);
    }
}