using Core.Domain.Exceptions;

namespace Academic.Domain.Exceptions;

/// <summary>
/// Thrown when GPA calculation is invalid
/// </summary>
public class InvalidGPACalculationException : DomainException
{
    public override string ErrorCode => "ACD010";
    public override int StatusCode => 400;

    public InvalidGPACalculationException(string message)
        : base($"Invalid GPA calculation: {message}") { }
}