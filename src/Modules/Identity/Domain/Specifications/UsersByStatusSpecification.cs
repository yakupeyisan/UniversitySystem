using Core.Domain.Specifications;
using Identity.Domain.Aggregates;
using Identity.Domain.Enums;

namespace Identity.Domain.Specifications;

public class UsersByStatusSpecification : Specification<User>
{
    public UsersByStatusSpecification(
        UserStatus status,
        int pageNumber,
        int pageSize) : this(status)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    public UsersByStatusSpecification(UserStatus status)
    {
        Criteria = u => !u.IsDeleted && u.Status == status;
        AddOrderBy(u => u.FirstName);
    }
}