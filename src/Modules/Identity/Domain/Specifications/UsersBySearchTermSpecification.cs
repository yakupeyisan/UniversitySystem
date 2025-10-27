using Core.Domain.Specifications;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Specifications;

public class UsersBySearchTermSpecification : Specification<User>
{
    public UsersBySearchTermSpecification(
        string searchTerm,
        int pageNumber,
        int pageSize) : this(searchTerm)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    public UsersBySearchTermSpecification(string searchTerm)
    {
        var term = searchTerm.ToLower();
        Criteria = u => !u.IsDeleted && (
            u.FirstName.ToLower().Contains(term) ||
            u.LastName.ToLower().Contains(term) ||
            u.Email.Value.Contains(term));

        AddOrderBy(u => u.FirstName);
    }
}