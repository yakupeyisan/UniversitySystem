using Core.Domain.Specifications;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Specifications;

/// <summary>
/// Belirli bir kullanýcýnýn aktif oturumlarýný döndürür
/// </summary>
public class ActiveLoginHistoriesSpecification : Specification<LoginHistory>
{
    public ActiveLoginHistoriesSpecification(Guid userId, int sessionTimeoutMinutes = 30)
    {
        var sessionExpiry = DateTime.UtcNow.AddMinutes(-sessionTimeoutMinutes);

        Criteria = lh => lh.UserId == userId
                         && lh.LogoutAt == null
                         && lh.LoginAt > sessionExpiry;

        AddOrderByDescending(lh => lh.LoginAt);
    }
}