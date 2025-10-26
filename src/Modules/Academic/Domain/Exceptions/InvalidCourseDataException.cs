using Core.Domain.Exceptions;

namespace Academic.Domain.Exceptions;

/// <summary>
/// Exception thrown when course data is invalid
/// </summary>
public class InvalidCourseDataException : DomainException
{
    public InvalidCourseDataException(string message)
        : base(message)
    {
    }

    public override string ErrorCode => "errors.course.invalid.data";
    public override int StatusCode => 400;
}