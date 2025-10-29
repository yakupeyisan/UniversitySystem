using Core.Domain.Exceptions;

namespace Identity.Domain.Exceptions;

public class RoleNotFoundException : DomainException
{
    public RoleNotFoundException(Guid roleId)
        : base($"Role with ID {roleId} not found")
    {
    }

    public RoleNotFoundException(string roleName)
        : base($"Role '{roleName}' not found")
    {
    }

    public override string ErrorCode => "IDT006";
    public override int StatusCode => 404;
}