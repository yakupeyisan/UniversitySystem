namespace Identity.Domain.Enums;

/// <summary>
/// Hesap kilitleme nedenleri
/// </summary>
public enum AccountLockoutReason
{
    /// <summary>
    /// Çok fazla baþarýsýz giriþ denemesi
    /// </summary>
    TooManyFailedLoginAttempts = 1,

    /// <summary>
    /// Admin tarafýndan kilitleme
    /// </summary>
    AdminLocked = 2,

    /// <summary>
    /// Þüpheli etkinlik tespit edildi
    /// </summary>
    SuspiciousActivity = 3,

    /// <summary>
    /// Ýkili Faktör Kimlik Doðrulama baþarýsýz
    /// </summary>
    TwoFactorAuthenticationFailure = 4,

    /// <summary>
    /// Kullanýcý parolasýný unuttum ve çok fazla deneme yaptý
    /// </summary>
    PasswordResetAttemptsExceeded = 5,

    /// <summary>
    /// Hesap verifikasyonu baþarýsýz
    /// </summary>
    EmailVerificationFailure = 6,

    /// <summary>
    /// Güvenlik politikasý ihlali
    /// </summary>
    SecurityPolicyViolation = 7
}