using Core.Domain.Exceptions;

namespace Academic.Domain.Exceptions;

/// <summary>
/// Exception thrown when student hasn't met course prerequisites
/// </summary>
public class PrerequisiteNotMetException : DomainException
{
    public Guid StudentId { get; }
    public Guid CourseId { get; }

    public PrerequisiteNotMetException(Guid studentId, Guid courseId)
        : base($"Student {studentId} has not met the prerequisites for course {courseId}.")
    {
        StudentId = studentId;
        CourseId = courseId;
    }

    public override string ErrorCode => "errors.prerequisite.not.met";
    public override int StatusCode => 409;
}
