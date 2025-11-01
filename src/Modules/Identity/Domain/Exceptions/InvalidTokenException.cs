using Core.Domain.Exceptions;
namespace Identity.Domain.Exceptions;
public class InvalidTokenException : DomainException
{
    public InvalidTokenException(string message = "Invalid or expired token")
        : base(message)
    {
    }
    public override string ErrorCode => "IDT008";
    public override int StatusCode => 401;
}