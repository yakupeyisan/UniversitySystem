using Core.Domain.Filtering;
using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Filtering;
namespace PersonMgmt.Domain.Specifications;
public class GetStudentsSpecification : BaseFilteredSpecification<Person>
{
    public GetStudentsSpecification(string? filterString = null, int pageNumber = 1, int pageSize = 20)
    {
        AddInclude(p => p.Student);
        AddCriteria(p => p.Student != null && !p.Student.IsDeleted && !p.IsDeleted);
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