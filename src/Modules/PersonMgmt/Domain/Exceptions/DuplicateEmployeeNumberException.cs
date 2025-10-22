using Core.Domain.Exceptions;

namespace PersonMgmt.Domain.Exceptions;

/// <summary>
/// Employee number'ı zaten kullanımda olduğunda throw edilen exception
/// </summary>
public class DuplicateEmployeeNumberException : DomainException
{
    public string EmployeeNumber { get; }

    public DuplicateEmployeeNumberException(string employeeNumber)
        : base($"Employee number {employeeNumber} is already in use.")
    {
        EmployeeNumber = employeeNumber;
    }

    public override string ErrorCode => "errors.employee.number.duplicate";
    public override int StatusCode => 409;
}