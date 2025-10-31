using Core.Domain.Specifications;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Specifications;

/// <summary>
/// Kullan�c�n�n aktif 2FA token'lar�n� d�nd�r�r
/// </summary>
public class ActiveTwoFactorTokensSpecification : Specification<TwoFactorToken>
{
    public ActiveTwoFactorTokensSpecification(Guid userId)
    {
        Criteria = tft => tft.UserId == userId
                          && tft.IsActive
                          && tft.IsVerified
                          && tft.DisabledAt == null;

        AddOrderByDescending(tft => tft.VerifiedAt);
    }
}