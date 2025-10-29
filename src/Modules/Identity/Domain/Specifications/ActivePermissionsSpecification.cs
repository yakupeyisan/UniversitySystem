using Core.Domain.Specifications;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Specifications;

public class ActivePermissionsSpecification : Specification<Permission>
{

    public ActivePermissionsSpecification()
    {
        Criteria = p => p.IsActive;
        AddOrderBy(p => p.PermissionName);
    }
}