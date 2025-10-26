using Core.Domain.Exceptions;

namespace Academic.Domain.Exceptions;

/// <summary>
/// Exception thrown when a course is not found
/// </summary>
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
