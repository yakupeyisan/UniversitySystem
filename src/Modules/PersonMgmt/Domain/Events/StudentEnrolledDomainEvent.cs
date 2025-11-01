using Core.Domain.Events;
namespace PersonMgmt.Domain.Events;
public class StudentEnrolledDomainEvent : DomainEvent
{
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
    public Guid PersonId { get; set; }
    public string StudentNumber { get; set; }
    public int EducationLevel { get; set; }
    public DateTime EnrollmentDate { get; set; }
}