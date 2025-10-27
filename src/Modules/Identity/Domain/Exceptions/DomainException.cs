using Core.Domain.Exceptions;

namespace Identity.Domain.Exceptions;

public class InvalidCredentialsException : DomainException
{
    public override string ErrorCode => "IDT001";
    public override int StatusCode => 401;

    public InvalidCredentialsException(string message = "Invalid username or password")
        : base(message) { }
}

public class UserNotFoundException : DomainException
{
    public override string ErrorCode => "IDT002";
    public override int StatusCode => 404;

    public UserNotFoundException(Guid userId)
        : base($"User with ID {userId} not found") { }

    public UserNotFoundException(string email)
        : base($"User with email {email} not found") { }
}

public class UserAlreadyExistsException : DomainException
{
    public override string ErrorCode => "IDT003";
    public override int StatusCode => 409;

    public UserAlreadyExistsException(string email)
        : base($"User with email {email} already exists") { }
}

public class InvalidPasswordException : DomainException
{
    public override string ErrorCode => "IDT004";
    public override int StatusCode => 422;

    public InvalidPasswordException(string message = "Password does not meet security requirements")
        : base(message) { }
}

public class UnauthorizedAccessException : DomainException
{
    public override string ErrorCode => "IDT005";
    public override int StatusCode => 403;

    public UnauthorizedAccessException(string message = "You do not have permission to perform this action")
        : base(message) { }
}

public class RoleNotFoundException : DomainException
{
    public override string ErrorCode => "IDT006";
    public override int StatusCode => 404;

    public RoleNotFoundException(Guid roleId)
        : base($"Role with ID {roleId} not found") { }

    public RoleNotFoundException(string roleName)
        : base($"Role '{roleName}' not found") { }
}

public class PermissionNotFoundException : DomainException
{
    public override string ErrorCode => "IDT007";
    public override int StatusCode => 404;

    public PermissionNotFoundException(Guid permissionId)
        : base($"Permission with ID {permissionId} not found") { }

    public PermissionNotFoundException(string permissionName)
        : base($"Permission '{permissionName}' not found") { }
}

public class InvalidTokenException : DomainException
{
    public override string ErrorCode => "IDT008";
    public override int StatusCode => 401;

    public InvalidTokenException(string message = "Invalid or expired token")
        : base(message) { }
}

public class UserAccountLockedException : DomainException
{
    public override string ErrorCode => "IDT009";
    public override int StatusCode => 423;

    public UserAccountLockedException(Guid userId)
        : base($"User account {userId} is locked") { }
}