using Core.Domain;
using Core.Domain.Specifications;
using Identity.Domain.Enums;
using Identity.Domain.Events;
using Identity.Domain.Exceptions;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Aggregates;

public class User : AuditableEntity, ISoftDelete
{
    public Email Email { get; private set; }
    public PasswordHash PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public UserStatus Status { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockedUntil { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }

    private readonly List<Role> _roles = new();
    private readonly List<Permission> _permissions = new();
    private readonly List<RefreshToken> _refreshTokens = new();

    public IReadOnlyList<Role> Roles => _roles.AsReadOnly();
    public IReadOnlyList<Permission> Permissions => _permissions.AsReadOnly();
    public IReadOnlyList<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private User() { }

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

    public string FullName => $"{FirstName} {LastName}";

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

    public void ChangePassword(PasswordHash newPasswordHash)
    {
        if (newPasswordHash == null)
            throw new ArgumentNullException(nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
        FailedLoginAttempts = 0;
        LockedUntil = null;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserPasswordChangedEvent(Id, Email.Value));
    }

    public void VerifyEmail()
    {
        IsEmailVerified = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddRole(Role role)
    {
        if (role == null)
            throw new ArgumentNullException(nameof(role));

        if (_roles.Any(r => r.Id == role.Id))
            return;

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

    public void AddPermission(Permission permission)
    {
        if (permission == null)
            throw new ArgumentNullException(nameof(permission));

        if (_permissions.Any(p => p.Id == permission.Id))
            return;

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

    public bool HasPermission(Guid permissionId) =>
        _permissions.Any(p => p.Id == permissionId) ||
        _roles.Any(r => r.HasPermission(permissionId));

    public bool HasRole(Guid roleId) => _roles.Any(r => r.Id == roleId);

    public void RecordSuccessfulLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;
        LockedUntil = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordFailedLoginAttempt()
    {
        FailedLoginAttempts++;
        UpdatedAt = DateTime.UtcNow;

        // Lock account after 5 failed attempts
        if (FailedLoginAttempts >= 5)
        {
            LockedUntil = DateTime.UtcNow.AddMinutes(30);
            Status = UserStatus.Locked;
        }
    }

    public void UnlockAccount()
    {
        FailedLoginAttempts = 0;
        LockedUntil = null;
        Status = UserStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

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
    }

    public void Deactivate()
    {
        SetStatus(UserStatus.Inactive);
    }

    public void AddRefreshToken(RefreshToken token)
    {
        if (token == null)
            throw new ArgumentNullException(nameof(token));

        // Remove expired tokens
        _refreshTokens.RemoveAll(t => t.IsExpired);

        _refreshTokens.Add(token);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RevokeRefreshToken(string token, string reason = "")
    {
        var refreshToken = _refreshTokens.FirstOrDefault(t => t.Token == token);
        if (refreshToken != null && !refreshToken.IsRevoked)
        {
            refreshToken.Revoke(reason);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public RefreshToken GetValidRefreshToken(string token)
    {
        var refreshToken = _refreshTokens.FirstOrDefault(t => t.Token == token);
        if (refreshToken == null || !refreshToken.IsValid)
            throw new InvalidTokenException("Refresh token is invalid or expired");

        return refreshToken;
    }

    public void RevokeAllRefreshTokens()
    {
        foreach (var token in _refreshTokens.Where(t => !t.IsRevoked))
        {
            token.Revoke("Revoked all tokens");
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete(Guid deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserDeletedEvent(Id, Email.Value));
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsLocked => Status == UserStatus.Locked && LockedUntil.HasValue && LockedUntil.Value > DateTime.UtcNow;

    public bool IsActive => Status == UserStatus.Active && !IsDeleted;
}