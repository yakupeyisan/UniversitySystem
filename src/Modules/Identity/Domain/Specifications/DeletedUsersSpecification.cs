using Core.Domain.Specifications;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Specifications;

public class DeletedUsersSpecification : Specification<User>
{
    public DeletedUsersSpecification(
        int pageNumber,
        int pageSize) : this()
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    public DeletedUsersSpecification()
    {
        Criteria = u => u.IsDeleted;
        AddOrderBy(u => u.DeletedAt);
    }
}