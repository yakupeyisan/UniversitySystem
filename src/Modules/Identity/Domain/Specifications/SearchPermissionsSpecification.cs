using Core.Domain.Specifications;
using Identity.Domain.Aggregates;
namespace Identity.Domain.Specifications;
public class SearchPermissionsSpecification : Specification<Permission>
{
    public SearchPermissionsSpecification(string searchTerm)
    {
        var lowerSearchTerm = searchTerm?.ToLower() ?? string.Empty;
        Criteria = p => p.IsActive &&
                        (p.PermissionName.ToLower().Contains(lowerSearchTerm) ||
                         p.Description.ToLower().Contains(lowerSearchTerm));
        AddOrderBy(p => p.PermissionName);
    }
}