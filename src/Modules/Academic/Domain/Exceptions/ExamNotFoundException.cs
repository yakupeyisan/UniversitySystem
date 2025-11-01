using Core.Domain.Exceptions;
namespace Academic.Domain.Exceptions;
public class ExamNotFoundException : DomainException
{
    public ExamNotFoundException(Guid examId)
        : base($"Exam with ID {examId} was not found.")
    {
        ExamId = examId;
    }
    public Guid ExamId { get; }
    public override string ErrorCode => "errors.exam.not.found";
    public override int StatusCode => 404;
}