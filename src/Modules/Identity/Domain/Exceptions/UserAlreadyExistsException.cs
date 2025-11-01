using Core.Domain.Exceptions;
namespace Identity.Domain.Exceptions;
public class UserAlreadyExistsException : DomainException
{
    public UserAlreadyExistsException(string email)
        : base($"User with email {email} already exists")
    {
    }
    public override string ErrorCode => "IDT003";
    public override int StatusCode => 409;
}