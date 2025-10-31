namespace Identity.Domain.Enums;

/// <summary>
/// 2FA (Ýki Faktörlü Kimlik Doðrulama) yöntemleri
/// </summary>
public enum TwoFactorMethod
{
    /// <summary>
    /// Google Authenticator, Microsoft Authenticator, vb.
    /// </summary>
    AuthenticatorApp = 1,

    /// <summary>
    /// SMS ile kod gönderimi
    /// </summary>
    Sms = 2,

    /// <summary>
    /// E-posta ile kod gönderimi
    /// </summary>
    Email = 3,

    /// <summary>
    /// Telefonla arama
    /// </summary>
    PhoneCall = 4,

    /// <summary>
    /// Windows Hello / Biometrik
    /// </summary>
    WindowsHello = 5,

    /// <summary>
    /// FIDO2 / Security Key
    /// </summary>
    Fido2 = 6
}