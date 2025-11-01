using Core.Domain.Specifications;
using Identity.Domain.Aggregates;
namespace Identity.Domain.Specifications;
public class PermissionByNameSpecification : Specification<Permission>
{
    public PermissionByNameSpecification(string name)
    {
        Criteria = p => p.PermissionName.ToLower() == name.ToLower();
    }
}