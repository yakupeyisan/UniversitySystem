using Core.Domain.Exceptions;
namespace Academic.Domain.Exceptions;
public class InvalidCourseDataException : DomainException
{
    public InvalidCourseDataException(string message)
        : base(message)
    {
    }
    public override string ErrorCode => "errors.course.invalid.data";
    public override int StatusCode => 400;
}