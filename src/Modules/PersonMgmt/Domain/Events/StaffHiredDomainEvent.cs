using Core.Domain.Events;

namespace PersonMgmt.Domain.Events;

public class StaffHiredDomainEvent : DomainEvent
{
    public StaffHiredDomainEvent(
        Guid personId,
        string employeeNumber,
        int academicTitle,
        DateTime hireDate) : base(personId)
    {
        PersonId = personId;
        EmployeeNumber = employeeNumber;
        AcademicTitle = academicTitle;
        HireDate = hireDate;
    }

    public Guid PersonId { get; set; }
    public string EmployeeNumber { get; set; }
    public int AcademicTitle { get; set; }
    public DateTime HireDate { get; set; }
}