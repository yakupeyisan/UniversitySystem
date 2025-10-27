using Core.Domain.Specifications;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Specifications;

public class SystemRolesSpecification : Specification<Role>
{
    public SystemRolesSpecification(
        int pageNumber,
        int pageSize) : this()
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }
    public SystemRolesSpecification()
    {
        Criteria = r => r.IsSystemRole;
        AddOrderBy(r => r.RoleName);
    }
}