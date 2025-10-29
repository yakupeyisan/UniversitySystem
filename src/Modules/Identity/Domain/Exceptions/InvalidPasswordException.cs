using Core.Domain.Exceptions;

namespace Identity.Domain.Exceptions;

public class InvalidPasswordException : DomainException
{
    public InvalidPasswordException(string message = "Password does not meet security requirements")
        : base(message)
    {
    }

    public override string ErrorCode => "IDT004";
    public override int StatusCode => 422;
}