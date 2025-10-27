using Core.Domain.Specifications;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Specifications;

public class ValidRefreshTokensSpecification : Specification<RefreshToken>
{
    public ValidRefreshTokensSpecification(Guid userId)
    {
        Criteria = rt => rt.UserId == userId &&
                         rt.ExpiryDate > DateTime.UtcNow &&
                         !rt.IsRevoked;
        AddOrderByDescending(rt => rt.CreatedAt);
    }
}