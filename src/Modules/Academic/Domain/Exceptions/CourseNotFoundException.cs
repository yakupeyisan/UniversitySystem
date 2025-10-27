using Core.Domain.Exceptions;
namespace Academic.Domain.Exceptions;
public class CourseNotFoundException : DomainException
{
    public Guid CourseId { get; }
    public CourseNotFoundException(Guid courseId)
        : base($"Course with ID {courseId} was not found.")
    {
        CourseId = courseId;
    }
    public CourseNotFoundException(string courseCode)
        : base($"Course with code '{courseCode}' was not found.")
    {
    }
    public override string ErrorCode => "errors.course.not.found";
    public override int StatusCode => 404;
}