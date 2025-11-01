using Core.Domain.Specifications;
using Identity.Domain.Aggregates;
namespace Identity.Domain.Specifications;
public class DeletedUsersSpecification : Specification<User>
{
    public DeletedUsersSpecification()
    {
        Criteria = u => u.IsDeleted;
        AddOrderBy(u => u.DeletedAt);
    }
}