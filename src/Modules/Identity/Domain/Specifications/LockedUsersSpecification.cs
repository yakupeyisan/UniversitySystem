using Core.Domain.Specifications;
using Identity.Domain.Aggregates;
using Identity.Domain.Enums;

namespace Identity.Domain.Specifications;

public class LockedUsersSpecification : Specification<User>
{
    public LockedUsersSpecification(
        int pageNumber,
        int pageSize) : this()
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    public LockedUsersSpecification()
    {
        Criteria = u => !u.IsDeleted && u.Status == UserStatus.Locked;
        AddOrderBy(u => u.FirstName);
    }
}