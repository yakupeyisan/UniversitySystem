using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;

namespace PersonMgmt.Domain.Specifications;

public class PersonsByRestrictionLevelSpecification : Specification<Person>
{
    public PersonsByRestrictionLevelSpecification(RestrictionLevel restrictionLevel)
    {
        Criteria = p => !p.IsDeleted &&
                        p.Restrictions.Any(r => !r.IsDeleted &&
                                                r.RestrictionLevel == restrictionLevel &&
                                                r.IsActive &&
                                                r.StartDate <= DateTime.UtcNow &&
                                                (r.EndDate == null || r.EndDate >= DateTime.UtcNow));
        AddOrderBy(p => p.Name);
    }
}