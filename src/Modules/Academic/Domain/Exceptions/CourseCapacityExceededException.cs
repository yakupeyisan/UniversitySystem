using Core.Domain.Exceptions;

namespace Academic.Domain.Exceptions;

/// <summary>
/// Thrown when course capacity is exceeded
/// </summary>
public class CourseCapacityExceededException : DomainException
{
    public override string ErrorCode => "ACD002";
    public override int StatusCode => 409;

    public CourseCapacityExceededException(Guid courseId, int maxCapacity)
        : base($"Course {courseId} has reached its maximum capacity of {maxCapacity} students.") { }
}