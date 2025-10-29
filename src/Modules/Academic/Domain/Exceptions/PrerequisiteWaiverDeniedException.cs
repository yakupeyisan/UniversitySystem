using Core.Domain.Exceptions;

namespace Academic.Domain.Exceptions;

public class PrerequisiteWaiverDeniedException : DomainException
{
    public PrerequisiteWaiverDeniedException(Guid studentId, Guid courseId)
        : base($"Prerequisite waiver denied for student {studentId} for course {courseId}.")
    {
    }

    public override string ErrorCode => "ACD006";
    public override int StatusCode => 403;
}