using Core.Domain.Exceptions;

namespace Academic.Domain.Exceptions;

/// <summary>
/// Thrown when a prerequisite waiver is denied
/// </summary>
public class PrerequisiteWaiverDeniedException : DomainException
{
    public override string ErrorCode => "ACD006";
    public override int StatusCode => 403;

    public PrerequisiteWaiverDeniedException(Guid studentId, Guid courseId)
        : base($"Prerequisite waiver denied for student {studentId} for course {courseId}.") { }
}