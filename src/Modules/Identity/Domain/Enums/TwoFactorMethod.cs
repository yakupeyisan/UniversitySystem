namespace Identity.Domain.Enums;

/// <summary>
/// 2FA (�ki Fakt�rl� Kimlik Do�rulama) y�ntemleri
/// </summary>
public enum TwoFactorMethod
{
    /// <summary>
    /// Google Authenticator, Microsoft Authenticator, vb.
    /// </summary>
    AuthenticatorApp = 1,

    /// <summary>
    /// SMS ile kod g�nderimi
    /// </summary>
    Sms = 2,

    /// <summary>
    /// E-posta ile kod g�nderimi
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