namespace Academic.Application.DTOs;

public class CourseWaitingListEntryResponse
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public int Position { get; set; }
    public string Semester { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public bool IsAdmitted { get; set; }
    public DateTime? AdmissionDate { get; set; }
}