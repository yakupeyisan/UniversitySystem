namespace Identity.Domain.Enums;
public enum TwoFactorMethod
{
    AuthenticatorApp = 1,
    Sms = 2,
    Email = 3,
    PhoneCall = 4,
    WindowsHello = 5,
    Fido2 = 6
}