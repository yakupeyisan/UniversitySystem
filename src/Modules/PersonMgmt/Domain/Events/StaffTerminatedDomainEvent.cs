using Core.Domain.Events;
namespace PersonMgmt.Domain.Events;
public class StaffTerminatedDomainEvent : DomainEvent
{
    public Guid PersonId { get; set; }
    public string EmployeeNumber { get; set; }
    public DateTime TerminationDate { get; set; }
    public int YearsOfService { get; set; }
    public DateTime OccurredAt { get; set; }
    public StaffTerminatedDomainEvent(
        Guid personId,
        string employeeNumber,
        DateTime terminationDate,
        int yearsOfService,
        DateTime occurredAt) : base(personId)
    {
        PersonId = personId;
        EmployeeNumber = employeeNumber;
        TerminationDate = terminationDate;
        YearsOfService = yearsOfService;
        OccurredAt = occurredAt;
    }
}