using Core.Domain.Exceptions;
namespace Identity.Domain.Exceptions;
public class PermissionNotFoundException : DomainException
{
    public PermissionNotFoundException(Guid permissionId)
        : base($"Permission with ID {permissionId} not found")
    {
    }
    public PermissionNotFoundException(string permissionName)
        : base($"Permission '{permissionName}' not found")
    {
    }
    public override string ErrorCode => "IDT007";
    public override int StatusCode => 404;
}