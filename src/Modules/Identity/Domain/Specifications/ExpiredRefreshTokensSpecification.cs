using Core.Domain.Specifications;
using Identity.Domain.Aggregates;
namespace Identity.Domain.Specifications;
public class ExpiredRefreshTokensSpecification : Specification<RefreshToken>
{
    public ExpiredRefreshTokensSpecification()
    {
        Criteria = rt => rt.ExpiryDate <= DateTime.UtcNow;
        AddOrderByDescending(rt => rt.ExpiryDate);
    }
}