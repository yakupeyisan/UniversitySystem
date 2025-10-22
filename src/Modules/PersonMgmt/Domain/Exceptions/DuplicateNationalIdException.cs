using Core.Domain.Exceptions;

namespace PersonMgmt.Domain.Exceptions;

/// <summary>
/// National ID zaten kullanımda olduğunda throw edilen exception
/// </summary>
public class DuplicateNationalIdException : DomainException
{
    public string NationalId { get; }

    public DuplicateNationalIdException(string nationalId)
        : base($"National ID {nationalId} is already registered in the system.")
    {
        NationalId = nationalId;
    }

    public override string ErrorCode => "errors.person.nationalid.duplicate";
    public override int StatusCode => 409;
}