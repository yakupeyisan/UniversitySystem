namespace Identity.Domain.Enums;

/// <summary>
/// Giri� denemesi sonu�lar�
/// </summary>
public enum LoginResultType
{
    /// <summary>
    /// Ba�ar�l� giri�
    /// </summary>
    Success = 1,

    /// <summary>
    /// Ba�ar�s�z giri�
    /// </summary>
    Failed = 2,

    /// <summary>
    /// Hesap kilitli
    /// </summary>
    Locked = 3,

    /// <summary>
    /// E-posta do�rulanmam��
    /// </summary>
    EmailNotVerified = 4,

    /// <summary>
    /// Hesap inaktif
    /// </summary>
    InactiveAccount = 5,

    /// <summary>
    /// 2FA gerekli ama sa�lanmam��
    /// </summary>
    TwoFactorRequired = 6
}