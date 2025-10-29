using Core.Domain.Exceptions;

namespace Identity.Domain.Exceptions;

public class InvalidCredentialsException : DomainException
{
    public InvalidCredentialsException(string message = "Invalid username or password")
        : base(message)
    {
    }

    public override string ErrorCode => "IDT001";
    public override int StatusCode => 401;
}