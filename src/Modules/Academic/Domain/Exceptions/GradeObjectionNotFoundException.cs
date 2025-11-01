using Core.Domain.Exceptions;
namespace Academic.Domain.Exceptions;
public class GradeObjectionNotFoundException : DomainException
{
    public GradeObjectionNotFoundException(Guid objectionId)
        : base($"Grade objection with ID {objectionId} was not found.")
    {
        ObjectionId = objectionId;
    }
    public Guid ObjectionId { get; }
    public override string ErrorCode => "errors.grade.objection.not.found";
    public override int StatusCode => 404;
}