using Core.Domain;
using Core.Domain.Events;
using Core.Domain.Specifications;
using Identity.Domain.Enums;
using Identity.Domain.Events;
using Identity.Domain.ValueObjects;
namespace Identity.Domain.Aggregates;
public class User : AuditableEntity, ISoftDelete
{
    private string? _passwordResetCode;
    private DateTime? _passwordResetCodeExpiry;
    private User()
    {
    }
    public Email Email { get; private set; }
    public PasswordHash PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public UserStatus Status { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public bool IsActive => Status == UserStatus.Active && !IsDeleted;
    public bool IsLocked { get; private set; }
    public bool IsTwoFactorEnabled { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public DateTime? LastPasswordChangeAt { get; private set; }
    public int FailedLoginAttemptCount { get; private set; }
    public bool IsAccountLocked { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }
    public ICollection<Role> Roles { get; private set; } = new List<Role>();
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
    public ICollection<Permission> Permissions { get; private set; } = new List<Permission>();
    public ICollection<TwoFactorToken> TwoFactorTokens { get; private set; } = new List<TwoFactorToken>();
    public ICollection<LoginHistory> LoginHistories { get; private set; } = new List<LoginHistory>();
    public ICollection<FailedLoginAttempt> FailedLoginAttempts { get; private set; } = new List<FailedLoginAttempt>();
    public ICollection<UserAccountLockout> AccountLockouts { get; private set; } = new List<UserAccountLockout>();
    public static User Create(
    string email,
    string firstName,
    string lastName,
    string hashedPassword,
    string salt,
    Guid createdBy)
    {
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
        user.AddDomainEvent(new UserCreatedEvent(
            user.Id,
            user.Email.Value,
            user.FirstName,
            user.LastName));
        return user;
    }
    public void VerifyEmail()
    {
        if (IsEmailVerified)
            throw new InvalidOperationException("Email is already verified");
        IsEmailVerified = true;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new UserEmailVerifiedEvent(Id, Email.Value));
    }
    public void UnverifyEmail()
    {
        IsEmailVerified = false;
        UpdatedAt = DateTime.UtcNow;
    }
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
    public void UpdateStatus(UserStatus newStatus, Guid updatedBy)
    {
        if (Status == newStatus)
            return;
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
        AddDomainEvent(new UserStatusChangedEvent(Id, Status.ToString()));
    }
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
        foreach (var token in RefreshTokens.Where(rt => !rt.IsRevoked))
        {
            token.Revoke("User account deleted");
        }
        AddDomainEvent(new UserDeletedEvent(Id, Email.Value));
    }
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
    public void AddRole(Role role, Guid addedBy)
    {
        if (role == null)
            throw new ArgumentNullException(nameof(role));
        if (Roles.Any(r => r.Id == role.Id))
            return;
        Roles.Add(role);
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = addedBy;
        AddDomainEvent(new RoleAssignedToUserEvent(Id, role.Id, role.RoleName));
    }
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
    public void ClearRoles(Guid clearedBy)
    {
        Roles.Clear();
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = clearedBy;
    }
    public bool HasRole(string roleName)
    {
        return Roles.Any(r => r.RoleName.Equals(roleName, StringComparison.OrdinalIgnoreCase) && r.IsActive);
    }
    public void AddPermission(Permission permission)
    {
        if (permission == null)
            throw new ArgumentNullException(nameof(permission));
        if (Permissions.Any(p => p.Id == permission.Id))
            return;
        Permissions.Add(permission);
        UpdatedAt = DateTime.UtcNow;
    }
    public void RemovePermission(Guid permissionId)
    {
        var permission = Permissions.FirstOrDefault(p => p.Id == permissionId);
        if (permission == null)
            return;
        Permissions.Remove(permission);
        UpdatedAt = DateTime.UtcNow;
    }
    public bool HasPermission(Guid permissionId)
    {
        return Roles
            .Where(r => r.IsActive)
            .SelectMany(r => r.Permissions)
            .Any(p => p.Id == permissionId && p.IsActive);
    }
    public bool HasPermission(string permissionName)
    {
        return Roles
            .Where(r => r.IsActive)
            .SelectMany(r => r.Permissions)
            .Any(p => p.IsActive && p.PermissionName == permissionName);
    }
    public void EnableTwoFactor(TwoFactorMethod method, Guid enabledBy)
    {
        IsTwoFactorEnabled = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = enabledBy;
        AddDomainEvent(new TwoFactorEnabledEvent(Id, method));
    }
    public void DisableTwoFactor(Guid disabledBy)
    {
        IsTwoFactorEnabled = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = disabledBy;
    }
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
        if (FailedLoginAttemptCount >= 5)
        {
            AddDomainEvent(new ExcessiveFailedLoginAttemptsEvent(Id, Email.Value, FailedLoginAttemptCount));
        }
    }
    public void ResetFailedLoginAttempts()
    {
        FailedLoginAttemptCount = 0;
        UpdatedAt = DateTime.UtcNow;
    }
    public void LockAccount(Guid lockedBy, string reason = "Account locked")
    {
        if (IsAccountLocked)
            return;
        IsAccountLocked = true;
        IsLocked = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = lockedBy;
        AddDomainEvent(new UserLockedEvent(Id, reason));
    }
    public void UnlockAccount(Guid unlockedBy)
    {
        if (!IsAccountLocked)
            return;
        IsAccountLocked = false;
        IsLocked = false;
        FailedLoginAttemptCount = 0;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = unlockedBy;
        AddDomainEvent(new UserUnlockedEvent(Id, "Account unlocked"));
    }
    public bool IsCurrentlyLocked()
    {
        return IsAccountLocked || IsLocked || Status == UserStatus.Suspended || IsDeleted;
    }
    public bool CanLogin()
    {
        return !IsDeleted
               && Status == UserStatus.Active
               && !IsAccountLocked
               && !IsLocked
               && IsEmailVerified;
    }
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
    public bool ValidatePasswordResetCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return false;
        if (string.IsNullOrEmpty(_passwordResetCode) || _passwordResetCodeExpiry == null)
            return false;
        if (DateTime.UtcNow > _passwordResetCodeExpiry)
            return false;
        return code == _passwordResetCode;
    }
    public bool ValidateEmailVerificationCode(string code)
    {
        return !string.IsNullOrWhiteSpace(code) && code.Length >= 32;
    }
    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }
    public List<Role> GetActiveRoles()
    {
        return Roles.Where(r => r.IsActive).ToList();
    }
    public List<Permission> GetAllPermissions()
    {
        return Roles
            .Where(r => r.IsActive)
            .SelectMany(r => r.Permissions)
            .Where(p => p.IsActive)
            .Distinct()
            .ToList();
    }
    public List<TwoFactorToken> GetActiveTwoFactorTokens()
    {
        return TwoFactorTokens
            .Where(tft => tft.IsActive && tft.IsVerified && tft.DisabledAt == null)
            .ToList();
    }
    public LoginHistory? GetLastLoginHistory()
    {
        return LoginHistories
            .OrderByDescending(lh => lh.LoginAt)
            .FirstOrDefault();
    }
    public int GetFailedLoginAttemptsInLastNDays(int days = 7)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        return FailedLoginAttempts.Count(fla => fla.AttemptedAt > cutoffDate);
    }
    public List<UserAccountLockout> GetActiveLockouts()
    {
        return AccountLockouts
            .Where(ual => !ual.IsUnlocked &&
                          (ual.LockedUntil == null || ual.LockedUntil > DateTime.UtcNow))
            .ToList();
    }
    public void AddRefreshToken(RefreshToken refreshToken)
    {
        if (refreshToken == null)
            throw new ArgumentNullException(nameof(refreshToken));
        if (refreshToken.UserId != Id)
            throw new InvalidOperationException("Refresh token does not belong to this user");
        RefreshTokens.Add(refreshToken);
        UpdatedAt = DateTime.UtcNow;
    }
    public void UpdateRefreshToken(string newRefreshTokenValue)
    {
        if (string.IsNullOrWhiteSpace(newRefreshTokenValue))
            throw new ArgumentException("Refresh token value cannot be empty", nameof(newRefreshTokenValue));
        var activeToken = RefreshTokens.FirstOrDefault(rt => !rt.IsRevoked && !rt.IsExpired);
        if (activeToken != null)
        {
            activeToken.Revoke("Token refreshed");
        }
        var newToken = RefreshToken.Create(
            Id,
            newRefreshTokenValue,
            DateTime.UtcNow.AddDays(7),
            "",
            "");
        RefreshTokens.Add(newToken);
        UpdatedAt = DateTime.UtcNow;
    }
    public void RevokeAllRefreshTokens()
    {
        foreach (var token in RefreshTokens.Where(rt => !rt.IsRevoked))
        {
            token.Revoke("All tokens revoked - Password changed");
        }
        UpdatedAt = DateTime.UtcNow;
    }
    public void RevokeAllRefreshTokens(Guid revokedBy)
    {
        foreach (var token in RefreshTokens.Where(rt => !rt.IsRevoked))
        {
            token.Revoke("User account deleted");
        }
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = revokedBy;
    }
    public void ClearRefreshToken()
    {
        RefreshTokens.Clear();
        UpdatedAt = DateTime.UtcNow;
    }
    public void SetPasswordResetCode(string resetCode)
    {
        if (string.IsNullOrWhiteSpace(resetCode))
            throw new ArgumentException("Reset code cannot be empty", nameof(resetCode));
        _passwordResetCode = resetCode;
        _passwordResetCodeExpiry = DateTime.UtcNow.AddMinutes(30);
        UpdatedAt = DateTime.UtcNow;
    }
}