using Core.Domain.Exceptions;

namespace Academic.Domain.Exceptions;

/// <summary>
/// Thrown when a prerequisite is not met
/// </summary>
public class PrerequisiteNotMetException : DomainException
{
    public override string ErrorCode => "ACD005";
    public override int StatusCode => 400;

    public PrerequisiteNotMetException(Guid studentId, Guid prerequisiteCourseId, Guid courseId)
        : base($"Student {studentId} has not completed the prerequisite course {prerequisiteCourseId} for course {courseId}.") { }
}