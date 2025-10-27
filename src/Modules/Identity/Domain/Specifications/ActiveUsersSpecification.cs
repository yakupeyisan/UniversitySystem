using Core.Domain.Specifications;
using Identity.Domain.Aggregates;
using Identity.Domain.Enums;

namespace Identity.Domain.Specifications;

public class ActiveUsersSpecification : Specification<User>
{
    public ActiveUsersSpecification(
        int pageNumber,
        int pageSize) : this()
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    public ActiveUsersSpecification()
    {
        Criteria = u => !u.IsDeleted && u.Status == UserStatus.Active;
        AddOrderBy(u => u.FirstName);
        AddOrderBy(u => u.LastName);
    }
}