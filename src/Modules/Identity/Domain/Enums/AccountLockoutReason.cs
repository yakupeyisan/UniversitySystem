namespace Identity.Domain.Enums;

/// <summary>
/// Hesap kilitleme nedenleri
/// </summary>
public enum AccountLockoutReason
{
    /// <summary>
    /// �ok fazla ba�ar�s�z giri� denemesi
    /// </summary>
    TooManyFailedLoginAttempts = 1,

    /// <summary>
    /// Admin taraf�ndan kilitleme
    /// </summary>
    AdminLocked = 2,

    /// <summary>
    /// ��pheli etkinlik tespit edildi
    /// </summary>
    SuspiciousActivity = 3,

    /// <summary>
    /// �kili Fakt�r Kimlik Do�rulama ba�ar�s�z
    /// </summary>
    TwoFactorAuthenticationFailure = 4,

    /// <summary>
    /// Kullan�c� parolas�n� unuttum ve �ok fazla deneme yapt�
    /// </summary>
    PasswordResetAttemptsExceeded = 5,

    /// <summary>
    /// Hesap verifikasyonu ba�ar�s�z
    /// </summary>
    EmailVerificationFailure = 6,

    /// <summary>
    /// G�venlik politikas� ihlali
    /// </summary>
    SecurityPolicyViolation = 7
}