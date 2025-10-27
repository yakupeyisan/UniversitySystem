using Core.Domain.Exceptions;
namespace Academic.Domain.Exceptions;
public class CourseCapacityExceededException : DomainException
{
    public Guid CourseId { get; }
    public CourseCapacityExceededException(Guid courseId)
        : base($"Course capacity has been exceeded for course ID {courseId}.")
    {
        CourseId = courseId;
    }
    public override string ErrorCode => "errors.course.capacity.exceeded";
    public override int StatusCode => 409;
}