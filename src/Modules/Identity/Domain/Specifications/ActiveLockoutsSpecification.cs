using Core.Domain.Specifications;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Specifications;

/// <summary>
/// Aktif hesap kilitleme kayýtlarýný döndürür
/// </summary>
public class ActiveLockoutsSpecification : Specification<UserAccountLockout>
{
    public ActiveLockoutsSpecification(Guid userId)
    {
        Criteria = ual => ual.UserId == userId
                          && !ual.IsUnlocked
                          && (ual.LockedUntil == null || ual.LockedUntil > DateTime.UtcNow);

        AddOrderByDescending(ual => ual.LockedAt);
    }
}