namespace Academic.Application.DTOs;

public class CourseRegistrationResponse
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string Semester { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? DropDate { get; set; }
    public string? DropReason { get; set; }
    public bool IsRetake { get; set; }
    public Guid? GradeId { get; set; }
}