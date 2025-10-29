using Core.Domain.Specifications;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Specifications;

public class ActiveRolesSpecification : Specification<Role>
{

    public ActiveRolesSpecification()
    {
        Criteria = r => r.IsActive;
        AddOrderBy(r => r.RoleName);
    }
}