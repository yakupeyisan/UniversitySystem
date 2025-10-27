using Core.Domain;
using Identity.Domain.Enums;

namespace Identity.Domain.Aggregates;

public class Role : AuditableEntity
{
    public string RoleName { get; private set; }
    public RoleType RoleType { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsSystemRole { get; private set; }

    private readonly List<Permission> _permissions = new();
    public IReadOnlyList<Permission> Permissions => _permissions.AsReadOnly();

    private Role() { }

    public static Role Create(
        string roleName,
        RoleType roleType,
        string description = "",
        bool isSystemRole = false)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentException("Role name cannot be empty", nameof(roleName));

        return new Role
        {
            Id = Guid.NewGuid(),
            RoleName = roleName.Trim(),
            RoleType = roleType,
            Description = description?.Trim() ?? string.Empty,
            IsActive = true,
            IsSystemRole = isSystemRole,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void AddPermission(Permission permission)
    {
        if (permission == null)
            throw new ArgumentNullException(nameof(permission));

        if (_permissions.Any(p => p.Id == permission.Id))
            return; // Permission already exists

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

    public void ClearPermissions()
    {
        _permissions.Clear();
        UpdatedAt = DateTime.UtcNow;
    }

    public bool HasPermission(Guid permissionId) => _permissions.Any(p => p.Id == permissionId);

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (IsSystemRole)
            throw new InvalidOperationException("System roles cannot be deactivated");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string description)
    {
        Description = description?.Trim() ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }
}