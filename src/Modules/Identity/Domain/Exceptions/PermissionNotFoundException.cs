using Core.Domain.Exceptions;

namespace Identity.Domain.Exceptions;

public class PermissionNotFoundException : DomainException
{
    public override string ErrorCode => "IDT007";
    public override int StatusCode => 404;

    public PermissionNotFoundException(Guid permissionId)
        : base($"Permission with ID {permissionId} not found") { }

    public PermissionNotFoundException(string permissionName)
        : base($"Permission '{permissionName}' not found") { }
}