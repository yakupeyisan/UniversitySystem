using Core.Domain.Specifications;
using Identity.Domain.Aggregates;
namespace Identity.Domain.Specifications;
public class RoleByIdSpecification : Specification<Role>
{
    public RoleByIdSpecification(Guid id)
    {
        Criteria = r => r.Id == id;
        AddInclude(r => r.Permissions);
    }
}