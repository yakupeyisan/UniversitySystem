using Core.Domain.Exceptions;
namespace PersonMgmt.Domain.Exceptions;
public class PersonNotFoundException : DomainException
{
    public PersonNotFoundException(Guid personId)
        : base($"Person with ID {personId} was not found.")
    {
        PersonId = personId;
    }
    public Guid PersonId { get; }
    public override string ErrorCode => "errors.person.not.found";
    public override int StatusCode => 404;
}