using Core.Domain.Exceptions;

namespace PersonMgmt.Domain.Exceptions;

public class PersonIsNotStudentException : DomainException
{
    public PersonIsNotStudentException(Guid personId)
        : base($"Person with ID {personId} is not a student.")
    {
        PersonId = personId;
    }

    public Guid PersonId { get; }
    public override string ErrorCode => "errors.person.is.not.student";
    public override int StatusCode => 400;
}