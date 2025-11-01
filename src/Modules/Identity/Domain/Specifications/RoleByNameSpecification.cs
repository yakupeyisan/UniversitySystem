using Core.Domain.Specifications;
using Identity.Domain.Aggregates;
namespace Identity.Domain.Specifications;
public class RoleByNameSpecification : Specification<Role>
{
    public RoleByNameSpecification(string name)
    {
        Criteria = r => r.RoleName.ToLower() == name.ToLower();
        AddInclude(r => r.Permissions);
    }
}