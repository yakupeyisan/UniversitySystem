using Core.Domain.Exceptions;
namespace Academic.Domain.Exceptions;
public class InvalidGradeException : DomainException
{
    public override string ErrorCode => "ACD008";
    public override int StatusCode => 400;
    public InvalidGradeException(string message)
        : base($"Invalid grade: {message}") { }
}