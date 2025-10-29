using Core.Domain.Specifications;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Specifications;

public class ActiveRolesSpecification : Specification<Role>
{
    public ActiveRolesSpecification(
        int pageNumber,
        int pageSize) : this()
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    public ActiveRolesSpecification()
    {
        Criteria = r => r.IsActive;
        AddOrderBy(r => r.RoleName);
    }
}