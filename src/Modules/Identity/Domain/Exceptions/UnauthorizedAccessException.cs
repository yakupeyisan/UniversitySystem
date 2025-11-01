using Core.Domain.Exceptions;
namespace Identity.Domain.Exceptions;
public class UnauthorizedAccessException : DomainException
{
    public UnauthorizedAccessException(string message = "You do not have permission to perform this action")
        : base(message)
    {
    }
    public override string ErrorCode => "IDT005";
    public override int StatusCode => 403;
}