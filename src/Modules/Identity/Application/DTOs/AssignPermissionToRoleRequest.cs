namespace Identity.Application.DTOs;

public class AssignPermissionToRoleRequest
{
    public AssignPermissionToRoleRequest()
    {
    }

    public AssignPermissionToRoleRequest(Guid permissionId)
    {
        if (permissionId == Guid.Empty)
            throw new ArgumentException("Permission ID cannot be empty", nameof(permissionId));

        PermissionId = permissionId;
    }

    public Guid PermissionId { get; set; }
}