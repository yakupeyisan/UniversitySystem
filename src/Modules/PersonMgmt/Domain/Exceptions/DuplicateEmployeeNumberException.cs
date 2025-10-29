using Core.Domain.Exceptions;

namespace PersonMgmt.Domain.Exceptions;

public class DuplicateEmployeeNumberException : DomainException
{
    public DuplicateEmployeeNumberException(string employeeNumber)
        : base($"Employee with number {employeeNumber} already exists.")
    {
        EmployeeNumber = employeeNumber;
    }

    public string EmployeeNumber { get; }
    public override string ErrorCode => "errors.person.duplicate.employee.number";
    public override int StatusCode => 409;
}