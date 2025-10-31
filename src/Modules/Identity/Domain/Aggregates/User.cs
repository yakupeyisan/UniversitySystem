using Core.Domain;
using Core.Domain.Events;
using Core.Domain.Specifications;
using Identity.Domain.Enums;
using Identity.Domain.Events;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Aggregates;

/// <summary>
/// User Aggregate Root - Tüm kimlik doðrulama ve kullanýcý yönetimi iþlemlerini yönetir
/// DDD Pattern: Aggregate Root olarak tüm consistency'den sorumludur
/// ISoftDelete uygulanmýþ - soft delete iþlemleri için
/// </summary>
public class User : AuditableEntity, ISoftDelete
{
    // ============ Private Constructor ============
    /// <summary>
    /// EF Core tarafýndan kullanýlmak üzere private constructor
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
    /// Adý
    /// </summary>
    public string FirstName { get; private set; }

    /// <summary>
    /// Soyadý
    /// </summary>
    public string LastName { get; private set; }

    /// <summary>
    /// Kullanýcý durumu (Active, Inactive, Suspended, Deleted)
    /// </summary>
    public UserStatus Status { get; private set; }

    /// <summary>
    /// Email doðrulanmýþ mý
    /// </summary>
    public bool IsEmailVerified { get; private set; }

    // ============ SECURITY PROPERTIES ============

    /// <summary>
    /// Kullanýcý hesabý kilitli mi (brute force vs.)
    /// </summary>
    public bool IsLocked { get; private set; }

    /// <summary>
    /// 2FA (Ýki Faktörlü Kimlik Doðrulama) aktif mi
    /// </summary>
    public bool IsTwoFactorEnabled { get; private set; }

    /// <summary>
    /// Son baþarýlý giriþ tarihi
    /// </summary>
    public DateTime? LastLoginAt { get; private set; }

    /// <summary>
    /// Son parola deðiþikliði tarihi
    /// </summary>
    public DateTime? LastPasswordChangeAt { get; private set; }

    /// <summary>
    /// Baþarýsýz giriþ denemesi sayýsý (sýfýrlanýr baþarýlý giriþte)
    /// </summary>
    public int FailedLoginAttemptCount { get; private set; }

    /// <summary>
    /// Hesap kilitli mi (admin tarafýndan veya çok fazla baþarýsýz deneme)
    /// </summary>
    public bool IsAccountLocked { get; private set; }

    // ============ SOFT DELETE PROPERTIES ============
    /// <summary>
    /// Kullanýcý silinmiþ mi (Soft Delete)
    /// ISoftDelete interface'i tarafýndan talep edilen property
    /// </summary>
    public bool IsDeleted { get; private set; }

    /// <summary>
    /// Silindiði tarih (Soft Delete)
    /// Arþiv ve audit trail için önemli
    /// </summary>
    public DateTime? DeletedAt { get; private set; }

    /// <summary>
    /// Silen kullanýcý/admin ID'si (Soft Delete)
    /// Audit trail'de sorumluluk için önemli
    /// </summary>
    public Guid? DeletedBy { get; private set; }

    // ============ NAVIGATION PROPERTIES ============

    /// <summary>
    /// Kullanýcýnýn rolleri (Many-to-Many)
    /// </summary>
    public ICollection<Role> Roles { get; private set; } = new List<Role>();

    /// <summary>
    /// Refresh token'larý (One-to-Many)
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

    /// <summary>
    /// Ýzinler (Rollerinden türetilmiþ, navigation için)
    /// </summary>
    public ICollection<Permission> Permissions { get; private set; } = new List<Permission>();

    /// <summary>
    /// 2FA Token'larý (One-to-Many)
    /// </summary>
    public ICollection<TwoFactorToken> TwoFactorTokens { get; private set; } = new List<TwoFactorToken>();

    /// <summary>
    /// Giriþ geçmiþi kayýtlarý (One-to-Many)
    /// </summary>
    public ICollection<LoginHistory> LoginHistories { get; private set; } = new List<LoginHistory>();

    /// <summary>
    /// Baþarýsýz giriþ denemeleri (One-to-Many)
    /// </summary>
    public ICollection<FailedLoginAttempt> FailedLoginAttempts { get; private set; } = new List<FailedLoginAttempt>();

    /// <summary>
    /// Hesap kilitleme kayýtlarý (One-to-Many)
    /// </summary>
    public ICollection<UserAccountLockout> AccountLockouts { get; private set; } = new List<UserAccountLockout>();

    // ============ FACTORY METHODS ============

    /// <summary>
    /// Yeni kullanýcý oluþtur (Factory Method)
    /// DDD: Entity oluþturma iþ mantýðý aggregate'te kalýr
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
    /// Email doðrulanmýþ olarak iþaretle
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
    /// Email doðrulanmamýþ olarak geri al
    /// </summary>
    public void UnverifyEmail()
    {
        IsEmailVerified = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Parola deðiþtir
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
    /// Parola sýfýrla (Forgot Password workflow'u için)
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
    /// Kullanýcý durumunu güncelle
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
    /// Kullanýcý bilgilerini güncelle
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
    /// Kullanýcý sil (Soft Delete)
    /// ISoftDelete interface'i uygulamasý
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

        // Tüm refresh token'larý iptal et
        foreach (var token in RefreshTokens.Where(rt => !rt.IsRevoked))
        {
            token.Revoke("User account deleted");
        }

        AddDomainEvent(new UserDeletedEvent(Id, Email.Value));
    }

    /// <summary>
    /// Silinmiþ kullanýcýyý geri al
    /// ISoftDelete interface'i uygulamasý
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
    /// Role kaldýr
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
    /// Tüm rolleri temizle
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
    /// Belirli bir izne sahip mi kontrol et
    /// </summary>
    public bool HasPermission(Guid permissionId)
    {
        return Roles
            .Where(r => r.IsActive)
            .SelectMany(r => r.Permissions)
            .Any(p => p.Id == permissionId && p.IsActive);
    }

    // ============ SECURITY & 2FA METHODS ============

    /// <summary>
    /// 2FA'yý aktifleþtir
    /// </summary>
    public void EnableTwoFactor(TwoFactorMethod method, Guid enabledBy)
    {
        IsTwoFactorEnabled = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = enabledBy;
        AddDomainEvent(new TwoFactorEnabledEvent(Id, method));
    }

    /// <summary>
    /// 2FA'yý devre dýþý býrak
    /// </summary>
    public void DisableTwoFactor(Guid disabledBy)
    {
        IsTwoFactorEnabled = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = disabledBy;
    }

    /// <summary>
    /// Baþarýlý giriþ kaydý (login attempt'lerini sýfýrla ve son giriþ tarihini güncelle)
    /// </summary>
    public void RecordSuccessfulLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginAttemptCount = 0;
        IsAccountLocked = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Baþarýsýz giriþ denemesini artýr
    /// </summary>
    public void IncrementFailedLoginAttempts()
    {
        FailedLoginAttemptCount++;
        UpdatedAt = DateTime.UtcNow;

        // Eðer çok fazla deneme yapýldýysa, uyarý event'i gönder
        if (FailedLoginAttemptCount >= 5)
        {
            AddDomainEvent(new ExcessiveFailedLoginAttemptsEvent(Id, Email.Value, FailedLoginAttemptCount));
        }
    }

    /// <summary>
    /// Baþarýsýz giriþ denemelerini sýfýrla
    /// </summary>
    public void ResetFailedLoginAttempts()
    {
        FailedLoginAttemptCount = 0;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Hesabý kilitle (Admin veya otomatik olabilir)
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
    /// Hesabý aç
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
    /// Hesabýn kilitleme durumunu kontrol et
    /// </summary>
    public bool IsCurrentlyLocked()
    {
        return IsAccountLocked || IsLocked || Status == UserStatus.Suspended || IsDeleted;
    }

    /// <summary>
    /// Giriþ yapabilir mi (Security checks)
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
    /// Giriþ baþarýsýzlýðýnýn nedenini döndür
    /// </summary>
    public string GetLoginFailureReason()
    {
        if (IsDeleted)
            return "Kullanýcý hesabý silinmiþtir";

        if (Status != UserStatus.Active)
            return $"Kullanýcý hesabý {Status.ToString().ToLower()} durumdadýr";

        if (!IsEmailVerified)
            return "E-posta adresi doðrulanmamýþtýr";

        if (IsAccountLocked || IsLocked)
            return "Kullanýcý hesabý kilitlenmiþtir";

        return "Giriþ yapýlamaz";
    }

    // ============ VALIDATION METHODS ============

    /// <summary>
    /// Parola sýfýrlama kodu doðrulama
    /// </summary>
    public bool ValidatePasswordResetCode(string code)
    {
        // Gerçek uygulama: Redis'te saklanmýþ ve TTL olan kodu kontrol et
        return !string.IsNullOrWhiteSpace(code) && code.Length >= 32;
    }

    /// <summary>
    /// Email doðrulama kodu doðrulama
    /// </summary>
    public bool ValidateEmailVerificationCode(string code)
    {
        // Gerçek uygulama: Redis'te saklanmýþ ve TTL olan kodu kontrol et
        return !string.IsNullOrWhiteSpace(code) && code.Length >= 32;
    }

    // ============ QUERY METHODS ============

    /// <summary>
    /// Tam adýný döndür
    /// </summary>
    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }

    /// <summary>
    /// Tüm aktif rolleri döndür
    /// </summary>
    public List<Role> GetActiveRoles()
    {
        return Roles.Where(r => r.IsActive).ToList();
    }

    /// <summary>
    /// Tüm izinleri döndür (aktif rollerden)
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
    /// Belirli bir izni kontrol et (string olarak)
    /// </summary>
    public bool HasPermission(string permissionName)
    {
        return Roles
            .Where(r => r.IsActive)
            .SelectMany(r => r.Permissions)
            .Any(p => p.IsActive && p.PermissionName == permissionName);
    }

    /// <summary>
    /// Aktif 2FA token'larýný döndür
    /// </summary>
    public List<TwoFactorToken> GetActiveTwoFactorTokens()
    {
        return TwoFactorTokens
            .Where(tft => tft.IsActive && tft.IsVerified && tft.DisabledAt == null)
            .ToList();
    }

    /// <summary>
    /// Son giriþ geçmiþini döndür
    /// </summary>
    public LoginHistory? GetLastLoginHistory()
    {
        return LoginHistories
            .OrderByDescending(lh => lh.LoginAt)
            .FirstOrDefault();
    }

    /// <summary>
    /// Son N gün içindeki baþarýsýz deneme sayýsýný döndür
    /// </summary>
    public int GetFailedLoginAttemptsInLastNDays(int days = 7)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        return FailedLoginAttempts.Count(fla => fla.AttemptedAt > cutoffDate);
    }

    /// <summary>
    /// Aktif hesap kilitlemeleri döndür
    /// </summary>
    public List<UserAccountLockout> GetActiveLockouts()
    {
        return AccountLockouts
            .Where(ual => !ual.IsUnlocked &&
                          (ual.LockedUntil == null || ual.LockedUntil > DateTime.UtcNow))
            .ToList();
    }
}

// ============ ADDITIONAL DOMAIN EVENTS ============

/// <summary>
/// Email doðrulama event'i
/// </summary>
public class UserEmailVerifiedEvent : DomainEvent
{
    public UserEmailVerifiedEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; }
}

/// <summary>
/// Parola deðiþtirildi event'i
/// </summary>
public class UserPasswordChangedEvent : DomainEvent
{
    public UserPasswordChangedEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; }
}

/// <summary>
/// Parola sýfýrlandý event'i
/// </summary>
public class UserPasswordResetEvent : DomainEvent
{
    public UserPasswordResetEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; }
}

/// <summary>
/// Durumu deðiþtirildi event'i
/// </summary>
public class UserStatusChangedEvent : DomainEvent
{
    public UserStatusChangedEvent(Guid userId, string newStatus)
    {
        UserId = userId;
        NewStatus = newStatus;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public string NewStatus { get; }
    public DateTime OccurredOn { get; }
}

/// <summary>
/// Kullanýcý silindi event'i
/// </summary>
public class UserDeletedEvent : DomainEvent
{
    public UserDeletedEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; }
}

/// <summary>
/// Silinen kullanýcý geri alýndý event'i
/// </summary>
public class UserRestoredEvent : DomainEvent
{
    public UserRestoredEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; }
}

/// <summary>
/// Role atandý event'i
/// </summary>
public class RoleAssignedToUserEvent : DomainEvent
{
    public RoleAssignedToUserEvent(Guid userId, Guid roleId, string roleName)
    {
        UserId = userId;
        RoleId = roleId;
        RoleName = roleName;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public Guid RoleId { get; }
    public string RoleName { get; }
    public DateTime OccurredOn { get; }
}

/// <summary>
/// Role kaldýrýldý event'i
/// </summary>
public class RoleRemovedFromUserEvent : DomainEvent
{
    public RoleRemovedFromUserEvent(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public Guid RoleId { get; }
    public DateTime OccurredOn { get; }
}

/// <summary>
/// Çok fazla baþarýsýz giriþ denemesi event'i
/// </summary>
public class ExcessiveFailedLoginAttemptsEvent : DomainEvent
{
    public ExcessiveFailedLoginAttemptsEvent(Guid userId, string email, int attemptCount)
    {
        UserId = userId;
        Email = email;
        AttemptCount = attemptCount;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public int AttemptCount { get; }
    public DateTime OccurredOn { get; }
}

/// <summary>
/// Kullanýcý hesabý kilitlendi event'i
/// </summary>
public class UserLockedEvent : DomainEvent
{
    public UserLockedEvent(Guid userId, string reason)
    {
        UserId = userId;
        Reason = reason;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public string Reason { get; }
    public DateTime OccurredOn { get; }
}

/// <summary>
/// Kullanýcý hesabý açýldý event'i
/// </summary>
public class UserUnlockedEvent : DomainEvent
{
    public UserUnlockedEvent(Guid userId, string reason)
    {
        UserId = userId;
        Reason = reason;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public string Reason { get; }
    public DateTime OccurredOn { get; }
}

/// <summary>
/// Kullanýcý güncellenmiþ event'i
/// </summary>
public class UserUpdatedEvent : DomainEvent
{
    public UserUpdatedEvent(Guid userId, string email, string firstName, string lastName)
    {
        UserId = userId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public DateTime OccurredOn { get; }
}