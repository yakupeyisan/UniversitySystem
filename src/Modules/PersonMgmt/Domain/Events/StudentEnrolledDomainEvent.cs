using Core.Domain.Events;
namespace PersonMgmt.Domain.Events;
public class StudentEnrolledDomainEvent : DomainEvent
{
    public Guid PersonId { get; set; }
    public string StudentNumber { get; set; }
    public int EducationLevel { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public StudentEnrolledDomainEvent(
        Guid personId,
        string studentNumber,
        int educationLevel,
        DateTime enrollmentDate) : base(personId)
    {
        PersonId = personId;
        StudentNumber = studentNumber;
        EducationLevel = educationLevel;
        EnrollmentDate = enrollmentDate;
    }
}
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