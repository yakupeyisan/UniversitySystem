namespace Identity.Domain.Enums;
public enum LoginResultType
{
    Success = 1,
    Failed = 2,
    Locked = 3,
    EmailNotVerified = 4,
    InactiveAccount = 5,
    TwoFactorRequired = 6
}