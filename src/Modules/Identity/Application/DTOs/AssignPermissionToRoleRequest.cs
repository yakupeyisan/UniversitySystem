namespace Identity.Application.DTOs;

public class AssignPermissionToRoleRequest
{
    public Guid PermissionId { get; set; }

    public AssignPermissionToRoleRequest() { }

    public AssignPermissionToRoleRequest(Guid permissionId)
    {
        if (permissionId == Guid.Empty)
            throw new ArgumentException("Permission ID cannot be empty", nameof(permissionId));

        PermissionId = permissionId;
    }
}