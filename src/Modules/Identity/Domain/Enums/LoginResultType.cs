namespace Identity.Domain.Enums;

/// <summary>
/// Giriþ denemesi sonuçlarý
/// </summary>
public enum LoginResultType
{
    /// <summary>
    /// Baþarýlý giriþ
    /// </summary>
    Success = 1,

    /// <summary>
    /// Baþarýsýz giriþ
    /// </summary>
    Failed = 2,

    /// <summary>
    /// Hesap kilitli
    /// </summary>
    Locked = 3,

    /// <summary>
    /// E-posta doðrulanmamýþ
    /// </summary>
    EmailNotVerified = 4,

    /// <summary>
    /// Hesap inaktif
    /// </summary>
    InactiveAccount = 5,

    /// <summary>
    /// 2FA gerekli ama saðlanmamýþ
    /// </summary>
    TwoFactorRequired = 6
}