using Core.Domain.Specifications;
using Identity.Domain.Aggregates;
using Identity.Domain.Enums;

namespace Identity.Domain.Specifications;

public class RoleByTypeSpecification : Specification<Role>
{
    public RoleByTypeSpecification(RoleType roleType)
    {
        Criteria = r => r.RoleType == roleType;
        AddOrderBy(r => r.RoleName);
    }
}