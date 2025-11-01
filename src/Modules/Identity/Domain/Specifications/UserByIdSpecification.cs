using Core.Domain.Specifications;
using Identity.Domain.Aggregates;
namespace Identity.Domain.Specifications;
public class UserByIdSpecification : Specification<User>
{
    public UserByIdSpecification(Guid id)
    {
        Criteria = u => u.Id == id && !u.IsDeleted;
        AddInclude(u => u.Roles);
        AddInclude(u => u.Permissions);
    }
}