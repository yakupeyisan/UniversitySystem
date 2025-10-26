using Core.Domain.Exceptions;

namespace Academic.Domain.Exceptions;

/// <summary>
/// Thrown when a grade value is invalid
/// </summary>
public class InvalidGradeException : DomainException
{
    public override string ErrorCode => "ACD008";
    public override int StatusCode => 400;

    public InvalidGradeException(string message)
        : base($"Invalid grade: {message}") { }
}