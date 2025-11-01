namespace PersonMgmt.Application.DTOs;
public class EnrollStudentRequest
{
    public string StudentNumber { get; set; }
    public Guid ProgramId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public byte EducationLevel { get; set; }
}