using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;

namespace PersonMgmt.Domain.Specifications;

/// <summary>
/// Specification - Aktif personeli getir
/// </summary>
public class ActiveStaffSpecification : Specification<Person>
{
    public ActiveStaffSpecification()
    {
        Criteria = p => !p.IsDeleted &&
                        p.Staff != null &&
                        p.Staff.IsActive;

        AddOrderBy(p => p.Name);
    }
}