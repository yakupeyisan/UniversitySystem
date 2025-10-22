using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;

namespace PersonMgmt.Domain.Specifications;

/// <summary>
/// Specification - Aktif kısıtlamaları olan kişileri getir
/// </summary>
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