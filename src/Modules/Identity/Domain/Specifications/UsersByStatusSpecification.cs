using Core.Domain.Specifications;
using Identity.Domain.Aggregates;
using Identity.Domain.Enums;
namespace Identity.Domain.Specifications;
public class UsersByStatusSpecification : Specification<User>
{
    public UsersByStatusSpecification(UserStatus status)
    {
        Criteria = u => !u.IsDeleted && u.Status == status;
        AddOrderBy(u => u.FirstName);
    }
}