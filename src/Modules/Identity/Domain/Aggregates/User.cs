using Core.Domain;
using Core.Domain.Specifications;
using Identity.Domain.Enums;
using Identity.Domain.Events;
using Identity.Domain.Exceptions;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Aggregates;
public class User : AuditableEntity, ISoftDelete
{
    // ============ Identity Properties ============
    public Email Email { get; private set; }
    public PasswordHash PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public UserStatus Status { get; private set; }

    // ============ Email Verification Properties ============
    public bool IsEmailVerified { get; private set; }
    public string? EmailVerificationCode { get; private set; }
    public DateTime? EmailVerificationCodeExpiry { get; private set; }

    // ============ Password Reset Properties ============
    public string? PasswordResetCode { get; private set; }
    public DateTime? PasswordResetCodeExpiry { get; private set; }
    public DateTime? LastPasswordChangeAt { get; private set; }

    // ============ Login & Access Properties ============
    public DateTime? LastLoginAt { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockedUntil { get; private set; }

    // ============ Soft Delete Properties ============
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }

    // ============ Collections ============
    private readonly List<Role> _roles = new();
    private readonly List<Permission> _permissions = new();
    private readonly List<RefreshToken> _refreshTokens = new();

    public IReadOnlyList<Role> Roles => _roles.AsReadOnly();
    public IReadOnlyList<Permission> Permissions => _permissions.AsReadOnly();
    public IReadOnlyList<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private User() { }

    // ============ Factory Method ============
    public static User Create(
        Email email,
        PasswordHash passwordHash,
        string firstName,
        string lastName)
    {
        if (email == null)
            throw new ArgumentNullException(nameof(email));

        if (passwordHash == null)
            throw new ArgumentNullException(nameof(passwordHash));

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Status = UserStatus.Active,
            IsEmailVerified = false,
            IsDeleted = false,
            FailedLoginAttempts = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        user.AddDomainEvent(new UserCreatedEvent(
            user.Id,
            user.Email.Value,
            user.FirstName,
            user.LastName));

        return user;
    }

    // ============ Properties ============
    public string FullName => $"{FirstName} {LastName}";

    // ============ Profile Management ============
    public void UpdateProfile(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserUpdatedEvent(Id, Email.Value, FirstName, LastName));
    }

    // ============ Password Management ============

    /// <summary>
    /// �ifreyi de�i�tir (kullan�c� taraf�ndan, eski �ifre do�ruland�ktan sonra)
    /// </summary>
    public void ChangePassword(PasswordHash newPasswordHash)
    {
        if (newPasswordHash == null)
            throw new ArgumentNullException(nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
        FailedLoginAttempts = 0;
        LockedUntil = null;
        LastPasswordChangeAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        // T�m refresh token'lar� iptal et (g�venlik i�in)
        RevokeAllRefreshTokens();

        AddDomainEvent(new UserPasswordChangedEvent(Id, Email.Value));
    }

    /// <summary>
    /// �ifre s�f�rlama kodu ayarla (forgot password ak���nda)
    /// </summary>
    public void SetPasswordResetCode(string resetCode)
    {
        if (string.IsNullOrWhiteSpace(resetCode))
            throw new ArgumentException("Reset code cannot be empty", nameof(resetCode));

        PasswordResetCode = resetCode;
        PasswordResetCodeExpiry = DateTime.UtcNow.AddHours(1); // 1 saat ge�erli
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new PasswordResetRequestedEvent(Id, Email.Value,PasswordResetCode));
    }

    /// <summary>
    /// �ifre s�f�rlama kodunu valide et
    /// </summary>
    public bool ValidatePasswordResetCode(string resetCode)
    {
        if (string.IsNullOrWhiteSpace(resetCode))
            return false;

        if (PasswordResetCode != resetCode)
            return false; // Kod e�le�miyor

        if (PasswordResetCodeExpiry == null || PasswordResetCodeExpiry < DateTime.UtcNow)
            return false; // Kod s�resi dolmu�

        return true;
    }

    /// <summary>
    /// �ifreyi s�f�rla (password reset ak���nda)
    /// </summary>
    public void ResetPassword(string passwordHash,string passwordSalt)
    {
        PasswordHash = new PasswordHash(passwordHash,passwordSalt);
        PasswordResetCode = null;
        PasswordResetCodeExpiry = null;
        LastPasswordChangeAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;
        LockedUntil = null;
        UpdatedAt = DateTime.UtcNow;

        // T�m refresh token'lar� iptal et (g�venlik i�in)
        RevokeAllRefreshTokens();

        AddDomainEvent(new PasswordResetEvent(Id, Email.Value));
    }

    // ============ Email Verification ============

    /// <summary>
    /// Email do�rulama kodu ayarla
    /// </summary>
    public void SetEmailVerificationCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Verification code cannot be empty", nameof(code));

        EmailVerificationCode = code;
        EmailVerificationCodeExpiry = DateTime.UtcNow.AddHours(24); // 24 saat ge�erli
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Email do�rulama kodunu valide et
    /// </summary>
    public bool ValidateEmailVerificationCode(string code)
    {
        if (IsEmailVerified)
            return false; // Zaten do�rulanm��

        if (string.IsNullOrWhiteSpace(code))
            return false;

        if (EmailVerificationCode != code)
            return false; // Kod e�le�miyor

        if (EmailVerificationCodeExpiry == null || EmailVerificationCodeExpiry < DateTime.UtcNow)
            return false; // Kod s�resi dolmu�

        return true;
    }

    /// <summary>
    /// Email'i do�rula
    /// </summary>
    public void VerifyEmail()
    {
        if (IsEmailVerified)
            throw new InvalidOperationException("Email is already verified");

        IsEmailVerified = true;
        EmailVerificationCode = null;
        EmailVerificationCodeExpiry = null;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserEmailVerifiedEvent(Id, Email.Value));
    }

    /// <summary>
    /// Email do�rulamas�n� temizle (email de�i�tirildi�inde)
    /// </summary>
    public void ClearEmailVerification()
    {
        IsEmailVerified = false;
        EmailVerificationCode = null;
        EmailVerificationCodeExpiry = null;
        UpdatedAt = DateTime.UtcNow;
    }

    // ============ Role Management ============

    public void AddRole(Role role)
    {
        if (role == null)
            throw new ArgumentNullException(nameof(role));

        if (_roles.Any(r => r.Id == role.Id))
            return; // Zaten ekli

        _roles.Add(role);
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserRoleAssignedEvent(Id, Email.Value, role.RoleName));
    }

    public void RemoveRole(Guid roleId)
    {
        var role = _roles.FirstOrDefault(r => r.Id == roleId);
        if (role != null)
        {
            _roles.Remove(role);
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new UserRoleRevokedEvent(Id, Email.Value, role.RoleName));
        }
    }

    public void ClearRoles()
    {
        _roles.Clear();
        UpdatedAt = DateTime.UtcNow;
    }

    public bool HasRole(Guid roleId) => _roles.Any(r => r.Id == roleId);

    public bool HasRole(string roleName) => _roles.Any(r => r.RoleName == roleName);

    // ============ Permission Management ============

    public void AddPermission(Permission permission)
    {
        if (permission == null)
            throw new ArgumentNullException(nameof(permission));

        if (_permissions.Any(p => p.Id == permission.Id))
            return; // Zaten ekli

        _permissions.Add(permission);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemovePermission(Guid permissionId)
    {
        var permission = _permissions.FirstOrDefault(p => p.Id == permissionId);
        if (permission != null)
        {
            _permissions.Remove(permission);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Kullan�c� veya roller arac�l���yla izne sahip mi
    /// </summary>
    public bool HasPermission(Guid permissionId) =>
        _permissions.Any(p => p.Id == permissionId) ||
        _roles.Any(r => r.HasPermission(permissionId));

    /// <summary>
    /// T�m izinleri al (direkt + rol arac�l���yla)
    /// </summary>
    public IEnumerable<Permission> GetAllPermissions()
    {
        var allPermissions = new HashSet<Permission>(_permissions);
        foreach (var permission in _roles.SelectMany(r => r.Permissions))
        {
            allPermissions.Add(permission);
        }
        return allPermissions;
    }

    // ============ Login & Account Lock ============

    /// <summary>
    /// Ba�ar�l� login kayd�
    /// </summary>
    public void RecordSuccessfulLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;
        LockedUntil = null;
        Status = UserStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Ba�ar�s�z login denemesini kaydet
    /// 5 deneme sonras� otomatik hesap kilitlenir
    /// </summary>
    public void RecordFailedLoginAttempt()
    {
        FailedLoginAttempts++;
        UpdatedAt = DateTime.UtcNow;

        // Lock account after 5 failed attempts
        if (FailedLoginAttempts >= 5)
        {
            LockedUntil = DateTime.UtcNow.AddMinutes(30);
            Status = UserStatus.Locked;
            AddDomainEvent(new UserLockedEvent(Id, Email.Value, LockedUntil.Value, "Too many failed login attempts"));
        }
    }

    /// <summary>
    /// Hesab� kilitle (admin taraf�ndan)
    /// </summary>
    public void LockAccount(string reason = "")
    {
        Status = UserStatus.Locked;
        LockedUntil = DateTime.UtcNow.AddDays(1); // 1 g�n boyunca kilitli
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserLockedEvent(Id, Email.Value, LockedUntil.Value, reason ?? "Locked by administrator"));
    }

    /// <summary>
    /// Hesab�n kilidini a�
    /// </summary>
    public void UnlockAccount()
    {
        FailedLoginAttempts = 0;
        LockedUntil = null;
        Status = UserStatus.Active;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserUnlockedEvent(Id, Email.Value));
    }

    /// <summary>
    /// Hesab�n kilitli olup olmad���n� kontrol et
    /// </summary>
    public bool IsAccountLocked => Status == UserStatus.Locked && LockedUntil.HasValue && LockedUntil.Value > DateTime.UtcNow;

    // ============ Status Management ============

    public void SetStatus(UserStatus newStatus)
    {
        if (Status == newStatus)
            return;

        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserStatusChangedEvent(Id, Email.Value, (int)Status));
    }

    public void Suspend()
    {
        SetStatus(UserStatus.Suspended);
    }

    public void Activate()
    {
        SetStatus(UserStatus.Active);
        LockedUntil = null;
        FailedLoginAttempts = 0;
    }

    public void Deactivate()
    {
        SetStatus(UserStatus.Inactive);
    }

    // ============ Refresh Token Management ============

    /// <summary>
    /// Refresh token ekle
    /// </summary>
    public void AddRefreshToken(RefreshToken token)
    {
        if (token == null)
            throw new ArgumentNullException(nameof(token));

        // S�resi ge�en token'lar� kald�r
        _refreshTokens.RemoveAll(t => t.IsExpired);

        // Maksimum 5 aktif token tutmak i�in eski olanlar� temizle
        var activeTokens = _refreshTokens.Where(t => !t.IsExpired && !t.IsRevoked).ToList();
        if (activeTokens.Count >= 5)
        {
            var oldestToken = activeTokens.OrderBy(t => t.CreatedAt).First();
            oldestToken.Revoke("New token issued, old token revoked");
        }

        _refreshTokens.Add(token);
        UpdatedAt = DateTime.UtcNow;
    }
    /// <summary>
    /// Updates the current refresh token
    /// </summary>
    public void UpdateRefreshToken(string newToken)
    {
        if (string.IsNullOrWhiteSpace(newToken))
            throw new ArgumentException("Token cannot be empty", nameof(newToken));

        AddRefreshToken(RefreshToken.Create(Id,newToken,
            DateTime.UtcNow.AddDays(7),"",""));
        UpdatedAt = DateTime.UtcNow;
    }
    public void ClearRefreshToken()
    {
        _refreshTokens.Where(t => !t.IsExpired && !t.IsRevoked).ToList().ForEach(activeToken =>
        {
            activeToken.Revoke("Cleared old tokens");
        });
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Refresh token'� iptal et
    /// </summary>
    public void RevokeRefreshToken(string token, string reason = "")
    {
        var refreshToken = _refreshTokens.FirstOrDefault(t => t.Token == token);
        if (refreshToken != null && !refreshToken.IsRevoked)
        {
            refreshToken.Revoke(reason ?? "Revoked");
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Ge�erli refresh token'� al
    /// </summary>
    public RefreshToken GetValidRefreshToken(string token)
    {
        var refreshToken = _refreshTokens.FirstOrDefault(t => t.Token == token);
        if (refreshToken == null || !refreshToken.IsValid)
            throw new InvalidTokenException("Refresh token is invalid or expired");

        return refreshToken;
    }

    /// <summary>
    /// T�m refresh token'lar� iptal et (logout all devices)
    /// </summary>
    public void RevokeAllRefreshTokens()
    {
        foreach (var token in _refreshTokens.Where(t => !t.IsRevoked))
        {
            token.Revoke("Revoked all tokens");
        }
        UpdatedAt = DateTime.UtcNow;
    }

    // ============ Soft Delete ============

    /// <summary>
    /// Soft delete - verileri kal�c� olarak silmez
    /// </summary>
    public void Delete(Guid deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        UpdatedAt = DateTime.UtcNow;

        // T�m refresh token'lar�n� iptal et
        RevokeAllRefreshTokens();

        AddDomainEvent(new UserDeletedEvent(Id, Email.Value));
    }

    /// <summary>
    /// Silinen hesab� geri y�kle
    /// </summary>
    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        UpdatedAt = DateTime.UtcNow;
    }

    // ============ Status Helpers ============

    /// <summary>
    /// Hesap aktif ve kullan�labilir mi
    /// </summary>
    public bool IsActive => Status == UserStatus.Active && !IsDeleted && !IsAccountLocked;

    /// <summary>
    /// Hesap login yapabilir mi
    /// </summary>
    public bool CanLogin => IsActive && IsEmailVerified;

    /// <summary>
    /// Profil �zeti al
    /// </summary>
    public (string Email, string FullName, bool IsActive, bool IsLocked, int RoleCount) GetProfile()
    {
        return (
            Email.Value,
            FullName,
            IsActive,
            IsAccountLocked,
            _roles.Count
        );
    }
    public bool IsLocked => Status == UserStatus.Locked && LockedUntil.HasValue && LockedUntil.Value > DateTime.UtcNow;

}