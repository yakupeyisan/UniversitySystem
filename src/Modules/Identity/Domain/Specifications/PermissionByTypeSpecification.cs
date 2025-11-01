using Core.Domain.Specifications;
using Identity.Domain.Aggregates;
using Identity.Domain.Enums;
namespace Identity.Domain.Specifications;
public class PermissionByTypeSpecification : Specification<Permission>
{
    public PermissionByTypeSpecification(PermissionType type)
    {
        Criteria = p => p.PermissionType == type;
        AddOrderBy(p => p.PermissionName);
    }
}