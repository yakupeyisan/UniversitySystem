using Core.Domain.Exceptions;

namespace Academic.Domain.Exceptions;

public class ExamConflictException : DomainException
{
    public ExamConflictException(Guid studentId, Guid examId1, Guid examId2)
        : base($"Student {studentId} has conflicting exams: {examId1} and {examId2}.")
    {
    }

    public ExamConflictException(string message)
        : base(message)
    {
    }

    public override string ErrorCode => "ACD007";
    public override int StatusCode => 409;
}