using Core.Domain.Exceptions;

namespace Academic.Domain.Exceptions;

/// <summary>
/// Exception thrown when a grade is not found
/// </summary>
public class GradeNotFoundException : DomainException
{
    public Guid GradeId { get; }

    public GradeNotFoundException(Guid gradeId)
        : base($"Grade with ID {gradeId} was not found.")
    {
        GradeId = gradeId;
    }

    public override string ErrorCode => "errors.grade.not.found";
    public override int StatusCode => 404;
}