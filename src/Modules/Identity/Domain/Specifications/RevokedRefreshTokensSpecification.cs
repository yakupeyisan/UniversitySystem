using Core.Domain.Specifications;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Specifications;

public class RevokedRefreshTokensSpecification : Specification<RefreshToken>
{
    public RevokedRefreshTokensSpecification(Guid userId)
    {
        Criteria = rt => rt.UserId == userId && rt.IsRevoked;
        AddOrderByDescending(rt => rt.CreatedAt);
    }
}