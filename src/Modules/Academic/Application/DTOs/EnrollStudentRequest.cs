namespace Academic.Application.DTOs;

public class EnrollStudentRequest
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string Semester { get; set; } = string.Empty;
    public bool IsRetake { get; set; }
    public Guid? PreviousGradeId { get; set; }
}