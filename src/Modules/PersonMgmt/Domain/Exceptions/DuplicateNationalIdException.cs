using Core.Domain.Exceptions;

namespace PersonMgmt.Domain.Exceptions;

/// <summary>
/// 🆕 NEW: Aynı National ID'ye sahip kişi zaten varsa throw edilen exception
/// </summary>
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