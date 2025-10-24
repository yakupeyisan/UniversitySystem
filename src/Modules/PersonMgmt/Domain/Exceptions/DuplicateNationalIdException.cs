using Core.Domain.Exceptions;
namespace PersonMgmt.Domain.Exceptions;
public class DuplicateNationalIdException : DomainException
{
    public string NationalId { get; }
    public DuplicateNationalIdException(string nationalId)
        : base($"Person with National ID {nationalId} already exists.")
    {
        NationalId = nationalId;
    }
    public override string ErrorCode => "errors.person.duplicate.national.id";
    public override int StatusCode => 409;
}