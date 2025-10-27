using Core.Domain.Specifications;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Specifications;

public class PermissionByIdSpecification : Specification<Permission>
{
    public PermissionByIdSpecification(Guid id)
    {
        Criteria = p => p.Id == id;
    }
}