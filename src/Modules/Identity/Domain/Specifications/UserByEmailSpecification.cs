using Core.Domain.Specifications;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Specifications;

public class UserByEmailSpecification : Specification<User>
{
    public UserByEmailSpecification(string email)
    {
        Criteria = u => u.Email.Value == email.ToLower() && !u.IsDeleted;
        AddInclude(u => u.Roles);
        AddInclude(u => u.Permissions);
    }
}