namespace Core.Application.Exceptions;
public class ConflictException : ApplicationException
{
    public override string ErrorCode => "BUSINESS_RULE_VIOLATION";
    public override int StatusCode => 409;
    public ConflictException(string message)
    : base(message)
    {
    }
    public ConflictException(string message, Exception innerException)
    : base(message, innerException)
    {
    }
    public static ConflictException FromDomainException(Exception domainException)
    {
        return new ConflictException(
            domainException.Message,
            domainException);
    }
}