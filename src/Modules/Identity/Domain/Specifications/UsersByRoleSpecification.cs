using Core.Domain.Specifications;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Specifications;

public class UsersByRoleSpecification : Specification<User>
{
    public UsersByRoleSpecification(
        Guid roleId,
        int pageNumber,
        int pageSize) : this(roleId)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    public UsersByRoleSpecification(Guid roleId)
    {
        Criteria = u => !u.IsDeleted && u.Roles.Any(r => r.Id == roleId);
        AddOrderBy(u => u.FirstName);
    }
}