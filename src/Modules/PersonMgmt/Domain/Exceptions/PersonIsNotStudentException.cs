using Core.Domain.Exceptions;

namespace PersonMgmt.Domain.Exceptions;

/// <summary>
/// 🆕 NEW: Person student değil iken student operasyonu yapılmaya çalışılırsa throw edilen exception
/// </summary>
public class PersonIsNotStudentException : DomainException
{
    public Guid PersonId { get; }

    public PersonIsNotStudentException(Guid personId)
        : base($"Person with ID {personId} is not a student.")
    {
        PersonId = personId;
    }

    public override string ErrorCode => "errors.person.is.not.student";
    public override int StatusCode => 400;
}