using Core.Domain.Events;

namespace PersonMgmt.Domain.Events;

/// <summary>
/// Personel işe alındığında raise edilen event
/// </summary>
public class StaffHiredDomainEvent : DomainEvent
{
    public Guid PersonId { get; set; }
    public string EmployeeNumber { get; set; }
    public int AcademicTitle { get; set; }
    public DateTime HireDate { get; set; }

    public StaffHiredDomainEvent(
        Guid personId,
        string employeeNumber,
        int academicTitle,
        DateTime hireDate)
    {
        PersonId = personId;
        EmployeeNumber = employeeNumber;
        AcademicTitle = academicTitle;
        HireDate = hireDate;
    }
}