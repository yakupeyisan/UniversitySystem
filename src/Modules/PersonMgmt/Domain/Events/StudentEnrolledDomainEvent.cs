using Core.Domain.Events;

namespace PersonMgmt.Domain.Events;

/// <summary>
/// Öğrenci kaydı yapılırken raise edilen event
/// </summary>
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