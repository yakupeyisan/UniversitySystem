using System.Linq.Expressions;
using Core.Domain.Filtering;
using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Filtering;

namespace PersonMgmt.Domain.Specifications;

public class GetPersonsWithFiltersSpecification : BaseFilteredSpecification<Person>
{
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

    public GetPersonsWithFiltersSpecification(string? filterString)
        : base(filterString, new FilterParser<Person>(null, new PersonFilterWhitelist()))
    {
        AddCriteria(x => !x.IsDeleted);
        AddIncludes(
            p => p.Student,
            p => p.Staff,
            p => p.HealthRecord,
            p => p.Restrictions
        );
        ApplyOrderBy(p => p.Name);
    }

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
        AddCriteria(x => !x.IsDeleted);
        AddIncludes(
            p => p.Student,
            p => p.Staff,
            p => p.HealthRecord,
            p => p.Restrictions
        );
        ApplyOrderBy(p => p.Name);
    }

    public GetPersonsWithFiltersSpecification(
        string? filterString,
        Expression<Func<Person, object>>? orderBy = null,
        bool descending = false)
        : base(filterString, new FilterParser<Person>(null, new PersonFilterWhitelist()))
    {
        AddCriteria(x => !x.IsDeleted);
        AddIncludes(
            p => p.Student,
            p => p.Staff,
            p => p.HealthRecord,
            p => p.Restrictions
        );
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
        AddCriteria(x => !x.IsDeleted);
        AddIncludes(
            p => p.Student,
            p => p.Staff,
            p => p.HealthRecord,
            p => p.Restrictions
        );
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

    public GetPersonsWithFiltersSpecification(
        string? filterString,
        IFilterParser<Person> customFilterParser,
        int pageNumber,
        int pageSize)
        : base(filterString, customFilterParser, pageNumber, pageSize)
    {
        AddCriteria(x => !x.IsDeleted);
        AddIncludes(
            p => p.Student,
            p => p.Staff,
            p => p.HealthRecord,
            p => p.Restrictions
        );
        ApplyOrderBy(p => p.Name);
    }
}