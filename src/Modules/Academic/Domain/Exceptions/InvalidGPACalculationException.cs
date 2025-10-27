using Core.Domain.Exceptions;
namespace Academic.Domain.Exceptions;
public class InvalidGPACalculationException : DomainException
{
    public override string ErrorCode => "ACD010";
    public override int StatusCode => 400;
    public InvalidGPACalculationException(string message)
        : base($"Invalid GPA calculation: {message}") { }
}