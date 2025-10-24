using Core.Domain.Exceptions;
namespace PersonMgmt.Domain.Exceptions;
public class DuplicateIdentificationNumberException : DomainException
{
    public string IdentificationNumber { get; }
    public DuplicateIdentificationNumberException(string identificationNumber)
        : base($"Person with National ID {identificationNumber} already exists.")
    {
        IdentificationNumber = identificationNumber;
    }
    public override string ErrorCode => "errors.person.duplicate.national.id";
    public override int StatusCode => 409;
}
