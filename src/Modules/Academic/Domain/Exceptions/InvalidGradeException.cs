using Core.Domain.Exceptions;

namespace Academic.Domain.Exceptions;

public class InvalidGradeException : DomainException
{
    public InvalidGradeException(string message)
        : base($"Invalid grade: {message}")
    {
    }

    public override string ErrorCode => "ACD008";
    public override int StatusCode => 400;
}