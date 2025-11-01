using Core.Domain.Specifications;
using Identity.Domain.Aggregates;
using Identity.Domain.Enums;
namespace Identity.Domain.Specifications;
public class PermissionsByTypeSpecification : Specification<Permission>
{
    public PermissionsByTypeSpecification(PermissionType permissionType)
    {
        Criteria = p => p.PermissionType == permissionType && p.IsActive;
        AddOrderBy(p => p.PermissionName);
    }
}