using Core.Domain.Exceptions;

namespace Academic.Domain.Exceptions;

/// <summary>
/// Thrown when a course is not found
/// </summary>
public class CourseNotFoundException : DomainException
{
    public override string ErrorCode => "ACD001";
    public override int StatusCode => 404;

    public CourseNotFoundException(Guid courseId)
        : base($"Course with ID {courseId} was not found.") { }

    public CourseNotFoundException(string courseCode)
        : base($"Course with code {courseCode} was not found.") { }
}