using Core.Domain.Exceptions;

namespace Identity.Domain.Exceptions;

public class UserNotFoundException : DomainException
{
    public UserNotFoundException(Guid userId)
        : base($"User with ID {userId} not found")
    {
    }

    public UserNotFoundException(string email)
        : base($"User with email {email} not found")
    {
    }

    public override string ErrorCode => "IDT002";
    public override int StatusCode => 404;
}