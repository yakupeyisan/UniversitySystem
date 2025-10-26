using Core.Domain.Exceptions;

namespace Academic.Domain.Exceptions;

/// <summary>
/// Thrown when there is an exam schedule conflict
/// </summary>
public class ExamConflictException : DomainException
{
    public override string ErrorCode => "ACD007";
    public override int StatusCode => 409;

    public ExamConflictException(Guid studentId, Guid examId1, Guid examId2)
        : base($"Student {studentId} has conflicting exams: {examId1} and {examId2}.") { }

    public ExamConflictException(string message)
        : base(message) { }
}