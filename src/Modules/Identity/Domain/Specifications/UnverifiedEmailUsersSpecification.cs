using Core.Domain.Specifications;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Specifications;

public class UnverifiedEmailUsersSpecification : Specification<User>
{
    public UnverifiedEmailUsersSpecification(
        int pageNumber,
        int pageSize) : this()
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    public UnverifiedEmailUsersSpecification()
    {
        Criteria = u => !u.IsDeleted && !u.IsEmailVerified;
        AddOrderBy(u => u.CreatedAt);
    }
}