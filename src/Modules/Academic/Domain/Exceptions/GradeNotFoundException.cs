using Core.Domain.Exceptions;

namespace Academic.Domain.Exceptions;

public class GradeNotFoundException : DomainException
{
    public GradeNotFoundException(Guid gradeId)
        : base($"Grade with ID {gradeId} was not found.")
    {
        GradeId = gradeId;
    }

    public Guid GradeId { get; }
    public override string ErrorCode => "errors.grade.not.found";
    public override int StatusCode => 404;
}