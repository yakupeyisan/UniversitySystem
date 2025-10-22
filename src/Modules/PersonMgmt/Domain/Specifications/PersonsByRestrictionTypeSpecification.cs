using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;

namespace PersonMgmt.Domain.Specifications;

/// <summary>
/// Specification - Belirli kısıtlama türüne sahip kişileri getir
/// </summary>
public class PersonsByRestrictionTypeSpecification : Specification<Person>
{
    public PersonsByRestrictionTypeSpecification(RestrictionType restrictionType)
    {
        Criteria = p => !p.IsDeleted &&
                        p.Restrictions.Any(r => !r.IsDeleted &&
                                                r.RestrictionType == restrictionType &&
                                                r.IsActive &&
                                                r.StartDate <= DateTime.UtcNow &&
                                                (r.EndDate == null || r.EndDate >= DateTime.UtcNow));

        AddOrderBy(p => p.Name);
    }
}