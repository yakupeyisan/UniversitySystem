using Core.Domain.Exceptions;

namespace Academic.Domain.Exceptions;

/// <summary>
/// Exception thrown when an exam is not found
/// </summary>
public class ExamNotFoundException : DomainException
{
    public Guid ExamId { get; }

    public ExamNotFoundException(Guid examId)
        : base($"Exam with ID {examId} was not found.")
    {
        ExamId = examId;
    }

    public override string ErrorCode => "errors.exam.not.found";
    public override int StatusCode => 404;
}