using Core.Domain.Exceptions;

namespace PersonMgmt.Domain.Exceptions;

/// <summary>
/// Student olarak işaretlenmiş bir kişi hakkında işlem yapılırken throw edilir
/// </summary>
public class PersonIsNotStudentException : DomainException
{
    public Guid PersonId { get; }

    public PersonIsNotStudentException(Guid personId)
        : base($"Person with ID {personId} is not a student.")
    {
        PersonId = personId;
    }

    public override string ErrorCode => "errors.person.not.student";
    public override int StatusCode => 400;
}