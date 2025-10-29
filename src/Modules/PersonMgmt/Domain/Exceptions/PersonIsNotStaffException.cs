using Core.Domain.Exceptions;

namespace PersonMgmt.Domain.Exceptions;

public class PersonIsNotStaffException : DomainException
{
    public PersonIsNotStaffException(Guid personId)
        : base($"Person with ID {personId} is not a staff member.")
    {
        PersonId = personId;
    }

    public Guid PersonId { get; }
    public override string ErrorCode => "errors.person.is.not.staff";
    public override int StatusCode => 400;
}