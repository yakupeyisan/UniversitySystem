using Core.Domain.Specifications;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Specifications;

public class ActivePermissionsSpecification : Specification<Permission>
{
    public ActivePermissionsSpecification(
        int pageNumber,
        int pageSize) : this()
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    public ActivePermissionsSpecification()
    {
        Criteria = p => p.IsActive;
        AddOrderBy(p => p.PermissionName);
    }
}