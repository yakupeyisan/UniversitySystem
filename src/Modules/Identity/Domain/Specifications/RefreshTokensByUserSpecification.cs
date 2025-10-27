using Core.Domain.Specifications;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Specifications;

public class RefreshTokensByUserSpecification : Specification<RefreshToken>
{
    public RefreshTokensByUserSpecification(Guid userId)
    {
        Criteria = rt => rt.UserId == userId;
        AddOrderByDescending(rt => rt.CreatedAt);
    }
}