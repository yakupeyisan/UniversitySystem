namespace Identity.Domain.Enums;
public enum AccountLockoutReason
{
    TooManyFailedLoginAttempts = 1,
    AdminLocked = 2,
    SuspiciousActivity = 3,
    TwoFactorAuthenticationFailure = 4,
    PasswordResetAttemptsExceeded = 5,
    EmailVerificationFailure = 6,
    SecurityPolicyViolation = 7
}