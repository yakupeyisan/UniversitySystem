using Core.Domain.Exceptions;

namespace Academic.Domain.Exceptions;

public class InvalidGPACalculationException : DomainException
{
    public InvalidGPACalculationException(string message)
        : base($"Invalid GPA calculation: {message}")
    {
    }

    public override string ErrorCode => "ACD010";
    public override int StatusCode => 400;
}