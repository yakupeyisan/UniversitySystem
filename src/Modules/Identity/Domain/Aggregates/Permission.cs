using Core.Domain;
using Identity.Domain.Enums;

namespace Identity.Domain.Aggregates;

public class Permission : AuditableEntity
{
    private Permission()
    {
    }

    public string PermissionName { get; private set; }
    public PermissionType PermissionType { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }

    public static Permission Create(
        string permissionName,
        PermissionType permissionType,
        string description = "")
    {
        if (string.IsNullOrWhiteSpace(permissionName))
            throw new ArgumentException("Permission name cannot be empty", nameof(permissionName));

        return new Permission
        {
            Id = Guid.NewGuid(),
            PermissionName = permissionName.Trim(),
            PermissionType = permissionType,
            Description = description?.Trim() ?? string.Empty,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string description)
    {
        Description = description?.Trim() ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePermission(string name, string description)
    {
        PermissionName = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }
}