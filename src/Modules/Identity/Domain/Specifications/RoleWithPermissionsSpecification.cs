using Core.Domain.Specifications;
using Identity.Domain.Aggregates;
namespace Identity.Domain.Specifications;
public class RoleWithPermissionsSpecification : Specification<Role>
{
    public RoleWithPermissionsSpecification(Guid roleId)
    {
        Criteria = r => r.Id == roleId;
        AddInclude(r => r.Permissions);
    }
}