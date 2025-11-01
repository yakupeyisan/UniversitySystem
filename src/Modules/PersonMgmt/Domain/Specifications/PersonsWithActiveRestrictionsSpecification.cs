using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;
namespace PersonMgmt.Domain.Specifications;
public class PersonsWithActiveRestrictionsSpecification : Specification<Person>
{
    public PersonsWithActiveRestrictionsSpecification()
    {
        Criteria = p => !p.IsDeleted &&
                        p.Restrictions.Any(r => !r.IsDeleted &&
                                                r.IsActive &&
                                                r.StartDate <= DateTime.UtcNow &&
                                                (r.EndDate == null || r.EndDate >= DateTime.UtcNow));
        AddOrderBy(p => p.Name);
    }
}