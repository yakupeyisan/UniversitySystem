using Core.Domain.Exceptions;

namespace Identity.Domain.Exceptions;

public class UserNotFoundException : DomainException
{
    public override string ErrorCode => "IDT002";
    public override int StatusCode => 404;

    public UserNotFoundException(Guid userId)
        : base($"User with ID {userId} not found") { }

    public UserNotFoundException(string email)
        : base($"User with email {email} not found") { }
}