using Core.Domain.Specifications;
using Identity.Domain.Aggregates;
using Identity.Domain.Enums;
namespace Identity.Domain.Specifications;
public class LockedUsersSpecification : Specification<User>
{
    public LockedUsersSpecification()
    {
        Criteria = u => !u.IsDeleted && u.Status == UserStatus.Locked;
        AddOrderBy(u => u.FirstName);
    }
}