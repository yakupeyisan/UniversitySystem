using Core.Domain.Exceptions;
namespace Academic.Domain.Exceptions;
public class GradeObjectionDeadlineExceededException : DomainException
{
    public override string ErrorCode => "ACD009";
    public override int StatusCode => 410;
    public GradeObjectionDeadlineExceededException(Guid gradeId)
        : base($"Grade objection deadline has passed for grade {gradeId}.") { }
}