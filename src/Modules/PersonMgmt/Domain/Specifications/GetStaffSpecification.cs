using Core.Domain.Filtering;
using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Filtering;

namespace PersonMgmt.Domain.Specifications;

public class GetStaffSpecification : BaseFilteredSpecification<Person>
{
    public GetStaffSpecification(string? filterString = null, int pageNumber = 1, int pageSize = 20)
    {
        AddInclude(p => p.Staff);
        AddCriteria(p => p.Staff != null && !p.Staff.IsDeleted && !p.IsDeleted);
        if (!string.IsNullOrWhiteSpace(filterString))
        {
            var filterParser = new FilterParser<Person>(
                whitelist: new PersonFilterWhitelist());
            var filterExpression = filterParser.Parse(filterString);
            AddCriteria(filterExpression);
        }

        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        AddOrderBy(p => p.CreatedAt);
    }
}