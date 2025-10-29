using Core.Domain.Specifications;
using Identity.Domain.Aggregates;
using Identity.Domain.Enums;

namespace Identity.Domain.Specifications;

public class PermissionByTypeSpecification : Specification<Permission>
{
    public PermissionByTypeSpecification(
        PermissionType type,
        int pageNumber,
        int pageSize) : this(type)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    public PermissionByTypeSpecification(PermissionType type)
    {
        Criteria = p => p.PermissionType == type;
        AddOrderBy(p => p.PermissionName);
    }
}