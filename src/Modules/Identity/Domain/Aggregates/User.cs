using Core.Domain;
using Core.Domain.Events;
using Core.Domain.Specifications;
using Identity.Domain.Enums;
using Identity.Domain.Events;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Aggregates;

/// <summary>
/// User Aggregate Root - T�m kimlik do�rulama ve kullan�c� y�netimi i�lemlerini y�netir
/// DDD Pattern: Aggregate Root olarak t�m consistency'den sorumludur
/// ISoftDelete uygulanm�� - soft delete i�lemleri i�in
/// </summary>
public class User : AuditableEntity, ISoftDelete
{
    // ============ Private Constructor ============
    private string? _passwordResetCode;
    private DateTime? _passwordResetCodeExpiry;

    /// <summary>
    /// EF Core taraf�ndan kullan�lmak �zere private constructor
    /// </summary>
    private User()
    {
    }

    // ============ Identity Properties ============

    /// <summary>
    /// Email adresi (ValueObject)
    /// </summary>
    public Email Email { get; private set; }

    /// <summary>
    /// Parola hash ve salt (ValueObject - Owned)
    /// </summary>
    public PasswordHash PasswordHash { get; private set; }

    /// <summary>
    /// Ad�
    /// </summary>
    public string FirstName { get; private set; }

    /// <summary>
    /// Soyad�
    /// </summary>
    public string LastName { get; private set; }

    /// <summary>
    /// Kullan�c� durumu (Active, Inactive, Suspended, Deleted)
    /// </summary>
    public UserStatus Status { get; private set; }

    /// <summary>
    /// Email do�rulanm�� m�
    /// </summary>
    public bool IsEmailVerified { get; private set; }

    public bool IsActive => Status == UserStatus.Active && !IsDeleted;

    // ============ SECURITY PROPERTIES ============

    /// <summary>
    /// Kullan�c� hesab� kilitli mi (brute force vs.)
    /// </summary>
    public bool IsLocked { get; private set; }

    /// <summary>
    /// 2FA (�ki Fakt�rl� Kimlik Do�rulama) aktif mi
    /// </summary>
    public bool IsTwoFactorEnabled { get; private set; }

    /// <summary>
    /// Son ba�ar�l� giri� tarihi
    /// </summary>
    public DateTime? LastLoginAt { get; private set; }

    /// <summary>
    /// Son parola de�i�ikli�i tarihi
    /// </summary>
    public DateTime? LastPasswordChangeAt { get; private set; }

    /// <summary>
    /// Ba�ar�s�z giri� denemesi say�s� (s�f�rlan�r ba�ar�l� giri�te)
    /// </summary>
    public int FailedLoginAttemptCount { get; private set; }

    /// <summary>
    /// Hesap kilitli mi (admin taraf�ndan veya �ok fazla ba�ar�s�z deneme)
    /// </summary>
    public bool IsAccountLocked { get; private set; }

    // ============ SOFT DELETE PROPERTIES ============
    /// <summary>
    /// Kullan�c� silinmi� mi (Soft Delete)
    /// ISoftDelete interface'i taraf�ndan talep edilen property
    /// </summary>
    public bool IsDeleted { get; private set; }

    /// <summary>
    /// Silindi�i tarih (Soft Delete)
    /// Ar�iv ve audit trail i�in �nemli
    /// </summary>
    public DateTime? DeletedAt { get; private set; }

    /// <summary>
    /// Silen kullan�c�/admin ID'si (Soft Delete)
    /// Audit trail'de sorumluluk i�in �nemli
    /// </summary>
    public Guid? DeletedBy { get; private set; }

    // ============ NAVIGATION PROPERTIES ============

    /// <summary>
    /// Kullan�c�n�n rolleri (Many-to-Many)
    /// </summary>
    public ICollection<Role> Roles { get; private set; } = new List<Role>();

    /// <summary>
    /// Refresh token'lar� (One-to-Many)
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

    /// <summary>
    /// �zinler (Rollerinden t�retilmi�, navigation i�in)
    /// </summary>
    public ICollection<Permission> Permissions { get; private set; } = new List<Permission>();

    /// <summary>
    /// 2FA Token'lar� (One-to-Many)
    /// </summary>
    public ICollection<TwoFactorToken> TwoFactorTokens { get; private set; } = new List<TwoFactorToken>();

    /// <summary>
    /// Giri� ge�mi�i kay�tlar� (One-to-Many)
    /// </summary>
    public ICollection<LoginHistory> LoginHistories { get; private set; } = new List<LoginHistory>();

    /// <summary>
    /// Ba�ar�s�z giri� denemeleri (One-to-Many)
    /// </summary>
    public ICollection<FailedLoginAttempt> FailedLoginAttempts { get; private set; } = new List<FailedLoginAttempt>();

    /// <summary>
    /// Hesap kilitleme kay�tlar� (One-to-Many)
    /// </summary>
    public ICollection<UserAccountLockout> AccountLockouts { get; private set; } = new List<UserAccountLockout>();

    // ============ FACTORY METHODS ============

    /// <summary>
    /// Yeni kullan�c� olu�tur (Factory Method)
    /// DDD: Entity olu�turma i� mant��� aggregate'te kal�r
    /// </summary>
    public static User Create(
        string email,
        string firstName,
        string lastName,
        string hashedPassword,
        string salt,
        Guid createdBy)
    {
        // Validasyon
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("Hashed password cannot be empty", nameof(hashedPassword));

        if (string.IsNullOrWhiteSpace(salt))
            throw new ArgumentException("Salt cannot be empty", nameof(salt));

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = new Email(email),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            PasswordHash = new PasswordHash(hashedPassword, salt),
            Status = UserStatus.Active,
            IsEmailVerified = false,
            IsLocked = false,
            IsTwoFactorEnabled = false,
            FailedLoginAttemptCount = 0,
            IsAccountLocked = false,
            IsDeleted = false,
            DeletedAt = null,
            DeletedBy = null,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = createdBy
        };

        // Domain event
        user.AddDomainEvent(new UserCreatedEvent(
            user.Id,
            user.Email.Value,
            user.FirstName,
            user.LastName));

        return user;
    }

    // ============ BUSINESS LOGIC METHODS ============

    /// <summary>
    /// Email do�rulanm�� olarak i�aretle
    /// </summary>
    public void VerifyEmail()
    {
        if (IsEmailVerified)
            throw new InvalidOperationException("Email is already verified");

        IsEmailVerified = true;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserEmailVerifiedEvent(Id, Email.Value));
    }

    /// <summary>
    /// Email do�rulanmam�� olarak geri al
    /// </summary>
    public void UnverifyEmail()
    {
        IsEmailVerified = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Parola de�i�tir
    /// </summary>
    public void ChangePassword(string newHashedPassword, string newSalt, Guid changedBy)
    {
        if (string.IsNullOrWhiteSpace(newHashedPassword))
            throw new ArgumentException("New password hash cannot be empty", nameof(newHashedPassword));

        if (string.IsNullOrWhiteSpace(newSalt))
            throw new ArgumentException("New salt cannot be empty", nameof(newSalt));

        PasswordHash = new PasswordHash(newHashedPassword, newSalt);
        LastPasswordChangeAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = changedBy;

        AddDomainEvent(new UserPasswordChangedEvent(Id, Email.Value));
    }

    /// <summary>
    /// Parola s�f�rla (Forgot Password workflow'u i�in)
    /// </summary>
    public void ResetPassword(string newHashedPassword, string newSalt, Guid resetBy)
    {
        if (string.IsNullOrWhiteSpace(newHashedPassword))
            throw new ArgumentException("New password hash cannot be empty", nameof(newHashedPassword));

        if (string.IsNullOrWhiteSpace(newSalt))
            throw new ArgumentException("New salt cannot be empty", nameof(newSalt));

        PasswordHash = new PasswordHash(newHashedPassword, newSalt);
        LastPasswordChangeAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = resetBy;

        AddDomainEvent(new UserPasswordResetEvent(Id, Email.Value));
    }

    /// <summary>
    /// Kullan�c� durumunu g�ncelle
    /// </summary>
    public void UpdateStatus(UserStatus newStatus, Guid updatedBy)
    {
        if (Status == newStatus)
            return;

        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new UserStatusChangedEvent(Id, Status.ToString()));
    }

    /// <summary>
    /// Kullan�c� bilgilerini g�ncelle
    /// </summary>
    public void UpdateProfile(string firstName, string lastName, Guid updatedBy)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new UserUpdatedEvent(Id, Email.Value, FirstName, LastName));
    }

    /// <summary>
    /// Kullan�c� sil (Soft Delete)
    /// ISoftDelete interface'i uygulamas�
    /// </summary>
    public void Delete(Guid deletedBy)
    {
        if (IsDeleted)
            throw new InvalidOperationException("User is already deleted");

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        Status = UserStatus.Deleted;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deletedBy;

        // T�m refresh token'lar� iptal et
        foreach (var token in RefreshTokens.Where(rt => !rt.IsRevoked))
        {
            token.Revoke("User account deleted");
        }

        AddDomainEvent(new UserDeletedEvent(Id, Email.Value));
    }

    /// <summary>
    /// Silinmi� kullan�c�y� geri al
    /// ISoftDelete interface'i uygulamas�
    /// </summary>
    public void Restore()
    {
        if (!IsDeleted)
            throw new InvalidOperationException("User is not deleted");

        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        Status = UserStatus.Active;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserRestoredEvent(Id, Email.Value));
    }

    /// <summary>
    /// Role ekle
    /// </summary>
    public void AddRole(Role role, Guid addedBy)
    {
        if (role == null)
            throw new ArgumentNullException(nameof(role));

        if (Roles.Any(r => r.Id == role.Id))
            return; // Already has this role

        Roles.Add(role);
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = addedBy;

        AddDomainEvent(new RoleAssignedToUserEvent(Id, role.Id, role.RoleName));
    }

    /// <summary>
    /// Role kald�r
    /// </summary>
    public void RemoveRole(Guid roleId, Guid removedBy)
    {
        var role = Roles.FirstOrDefault(r => r.Id == roleId);
        if (role == null)
            return;

        Roles.Remove(role);
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = removedBy;

        AddDomainEvent(new RoleRemovedFromUserEvent(Id, roleId));
    }

    /// <summary>
    /// T�m rolleri temizle
    /// </summary>
    public void ClearRoles(Guid clearedBy)
    {
        Roles.Clear();
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = clearedBy;
    }

    /// <summary>
    /// Belirli bir role sahip mi kontrol et
    /// </summary>
    public bool HasRole(string roleName)
    {
        return Roles.Any(r => r.RoleName.Equals(roleName, StringComparison.OrdinalIgnoreCase) && r.IsActive);
    }

    /// <summary>
    /// �zin ekle - DO�RUDAN KULLANI� (GrantPermissionCommand'dan)
    /// </summary>
    public void AddPermission(Permission permission)
    {
        if (permission == null)
            throw new ArgumentNullException(nameof(permission));

        if (Permissions.Any(p => p.Id == permission.Id))
            return; // Already has this permission

        Permissions.Add(permission);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// �zin kald�r - DO�RUDAN KULLANICI (RevokePermissionCommand'dan)
    /// </summary>
    public void RemovePermission(Guid permissionId)
    {
        var permission = Permissions.FirstOrDefault(p => p.Id == permissionId);
        if (permission == null)
            return;

        Permissions.Remove(permission);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Belirli bir izne sahip mi kontrol et
    /// </summary>
    public bool HasPermission(Guid permissionId)
    {
        return Roles
            .Where(r => r.IsActive)
            .SelectMany(r => r.Permissions)
            .Any(p => p.Id == permissionId && p.IsActive);
    }

    /// <summary>
    /// Belirli bir izni kontrol et (string olarak)
    /// </summary>
    public bool HasPermission(string permissionName)
    {
        return Roles
            .Where(r => r.IsActive)
            .SelectMany(r => r.Permissions)
            .Any(p => p.IsActive && p.PermissionName == permissionName);
    }
    // ============ SECURITY & 2FA METHODS ============

    /// <summary>
    /// 2FA'y� aktifle�tir
    /// </summary>
    public void EnableTwoFactor(TwoFactorMethod method, Guid enabledBy)
    {
        IsTwoFactorEnabled = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = enabledBy;
        AddDomainEvent(new TwoFactorEnabledEvent(Id, method));
    }

    /// <summary>
    /// 2FA'y� devre d��� b�rak
    /// </summary>
    public void DisableTwoFactor(Guid disabledBy)
    {
        IsTwoFactorEnabled = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = disabledBy;
    }

    /// <summary>
    /// Ba�ar�l� giri� kayd� (login attempt'lerini s�f�rla ve son giri� tarihini g�ncelle)
    /// </summary>
    public void RecordSuccessfulLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginAttemptCount = 0;
        IsAccountLocked = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordFailedLoginAttempt()
    {
        FailedLoginAttemptCount++;
        UpdatedAt = DateTime.UtcNow;

        // E�er �ok fazla deneme yap�ld�ysa, uyar� event'i g�nder
        if (FailedLoginAttemptCount >= 5)
        {
            AddDomainEvent(new ExcessiveFailedLoginAttemptsEvent(Id, Email.Value, FailedLoginAttemptCount));
        }
    }


    /// <summary>
    /// Ba�ar�s�z giri� denemelerini s�f�rla
    /// </summary>
    public void ResetFailedLoginAttempts()
    {
        FailedLoginAttemptCount = 0;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Hesab� kilitle (Admin veya otomatik olabilir)
    /// </summary>
    public void LockAccount(Guid lockedBy, string reason = "Account locked")
    {
        if (IsAccountLocked)
            return; // Already locked

        IsAccountLocked = true;
        IsLocked = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = lockedBy;

        AddDomainEvent(new UserLockedEvent(Id, reason));
    }

    /// <summary>
    /// Hesab� a�
    /// </summary>
    public void UnlockAccount(Guid unlockedBy)
    {
        if (!IsAccountLocked)
            return; // Not locked

        IsAccountLocked = false;
        IsLocked = false;
        FailedLoginAttemptCount = 0;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = unlockedBy;

        AddDomainEvent(new UserUnlockedEvent(Id, "Account unlocked"));
    }

    /// <summary>
    /// Hesab�n kilitleme durumunu kontrol et
    /// </summary>
    public bool IsCurrentlyLocked()
    {
        return IsAccountLocked || IsLocked || Status == UserStatus.Suspended || IsDeleted;
    }

    /// <summary>
    /// Giri� yapabilir mi (Security checks)
    /// </summary>
    public bool CanLogin()
    {
        return !IsDeleted
               && Status == UserStatus.Active
               && !IsAccountLocked
               && !IsLocked
               && IsEmailVerified;
    }

    /// <summary>
    /// Giri� ba�ar�s�zl���n�n nedenini d�nd�r
    /// </summary>
    public string GetLoginFailureReason()
    {
        if (IsDeleted)
            return "Kullan�c� hesab� silinmi�tir";

        if (Status != UserStatus.Active)
            return $"Kullan�c� hesab� {Status.ToString().ToLower()} durumdad�r";

        if (!IsEmailVerified)
            return "E-posta adresi do�rulanmam��t�r";

        if (IsAccountLocked || IsLocked)
            return "Kullan�c� hesab� kilitlenmi�tir";

        return "Giri� yap�lamaz";
    }

    // ============ VALIDATION METHODS ============

    /// <summary>
    /// Parola s�f�rlama kodu do�rulama
    /// </summary>
    public bool ValidatePasswordResetCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return false;

        // Kod bo� veya TTL ge�mi� ise false d�n
        if (string.IsNullOrEmpty(_passwordResetCode) || _passwordResetCodeExpiry == null)
            return false;

        // TTL kontrol
        if (DateTime.UtcNow > _passwordResetCodeExpiry)
            return false;

        // Kod e�le�me
        return code == _passwordResetCode;
    }

    /// <summary>
    /// Email do�rulama kodu do�rulama
    /// </summary>
    public bool ValidateEmailVerificationCode(string code)
    {
        // Ger�ek uygulama: Redis'te saklanm�� ve TTL olan kodu kontrol et
        return !string.IsNullOrWhiteSpace(code) && code.Length >= 32;
    }

    // ============ QUERY METHODS ============

    /// <summary>
    /// Tam ad�n� d�nd�r
    /// </summary>
    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }

    /// <summary>
    /// T�m aktif rolleri d�nd�r
    /// </summary>
    public List<Role> GetActiveRoles()
    {
        return Roles.Where(r => r.IsActive).ToList();
    }

    /// <summary>
    /// T�m izinleri d�nd�r (aktif rollerden)
    /// </summary>
    public List<Permission> GetAllPermissions()
    {
        return Roles
            .Where(r => r.IsActive)
            .SelectMany(r => r.Permissions)
            .Where(p => p.IsActive)
            .Distinct()
            .ToList();
    }

    /// <summary>
    /// Aktif 2FA token'lar�n� d�nd�r
    /// </summary>
    public List<TwoFactorToken> GetActiveTwoFactorTokens()
    {
        return TwoFactorTokens
            .Where(tft => tft.IsActive && tft.IsVerified && tft.DisabledAt == null)
            .ToList();
    }

    /// <summary>
    /// Son giri� ge�mi�ini d�nd�r
    /// </summary>
    public LoginHistory? GetLastLoginHistory()
    {
        return LoginHistories
            .OrderByDescending(lh => lh.LoginAt)
            .FirstOrDefault();
    }

    /// <summary>
    /// Son N g�n i�indeki ba�ar�s�z deneme say�s�n� d�nd�r
    /// </summary>
    public int GetFailedLoginAttemptsInLastNDays(int days = 7)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        return FailedLoginAttempts.Count(fla => fla.AttemptedAt > cutoffDate);
    }

    /// <summary>
    /// Aktif hesap kilitlemeleri d�nd�r
    /// </summary>
    public List<UserAccountLockout> GetActiveLockouts()
    {
        return AccountLockouts
            .Where(ual => !ual.IsUnlocked &&
                          (ual.LockedUntil == null || ual.LockedUntil > DateTime.UtcNow))
            .ToList();
    }

    /// <summary>
    /// Refresh token ekle
    /// LoginCommand'da �a�r�l�yor
    /// </summary>
    public void AddRefreshToken(RefreshToken refreshToken)
    {
        if (refreshToken == null)
            throw new ArgumentNullException(nameof(refreshToken));

        if (refreshToken.UserId != Id)
            throw new InvalidOperationException("Refresh token does not belong to this user");

        RefreshTokens.Add(refreshToken);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Refresh token'� g�ncelle
    /// RefreshTokenCommand'da �a�r�l�yor
    /// </summary>
    public void UpdateRefreshToken(string newRefreshTokenValue)
    {
        if (string.IsNullOrWhiteSpace(newRefreshTokenValue))
            throw new ArgumentException("Refresh token value cannot be empty", nameof(newRefreshTokenValue));

        // Mevcut token'lar� bulup g�ncelle
        var activeToken = RefreshTokens.FirstOrDefault(rt => !rt.IsRevoked && !rt.IsExpired);
        if (activeToken != null)
        {
            activeToken.Revoke("Token refreshed");
        }

        // Yeni token olu�tur
        var newToken = RefreshToken.Create(
            Id,
            newRefreshTokenValue,
            DateTime.UtcNow.AddDays(7),
            "", // IP Address - middleware'den set edilecek
            ""); // User Agent - middleware'den set edilecek

        RefreshTokens.Add(newToken);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// T�m refresh token'lar� iptal et
    /// ChangePasswordCommand'da �a�r�l�yor - g�venlik sebebiyle
    /// </summary>
    public void RevokeAllRefreshTokens()
    {
        foreach (var token in RefreshTokens.Where(rt => !rt.IsRevoked))
        {
            token.Revoke("All tokens revoked - Password changed");
        }

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// T�m refresh token'lar� iptal et (Delete'te kullan�lmak �zere)
    /// </summary>
    public void RevokeAllRefreshTokens(Guid revokedBy)
    {
        foreach (var token in RefreshTokens.Where(rt => !rt.IsRevoked))
        {
            token.Revoke("User account deleted");
        }

        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = revokedBy;
    }

    /// <summary>
    /// T�m refresh token'lar� temizle
    /// LogoutCommand'da �a�r�l�yor
    /// </summary>
    public void ClearRefreshToken()
    {
        RefreshTokens.Clear();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Parola s�f�rlama kodu set et
    /// ForgotPasswordCommand'da �a�r�l�yor
    /// 30 dakika ge�erli
    /// </summary>
    public void SetPasswordResetCode(string resetCode)
    {
        if (string.IsNullOrWhiteSpace(resetCode))
            throw new ArgumentException("Reset code cannot be empty", nameof(resetCode));

        _passwordResetCode = resetCode;
        _passwordResetCodeExpiry = DateTime.UtcNow.AddMinutes(30); // 30 dakika ge�erli
        UpdatedAt = DateTime.UtcNow;
    }
}