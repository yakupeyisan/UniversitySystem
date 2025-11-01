namespace PersonMgmt.Application.DTOs;
public class StudentResponse
{
    public Guid PersonId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public Guid ProgramId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public string EducationLevel { get; set; } = string.Empty;
    public string StudentStatus { get; set; } = string.Empty;
    public decimal? GPA { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}