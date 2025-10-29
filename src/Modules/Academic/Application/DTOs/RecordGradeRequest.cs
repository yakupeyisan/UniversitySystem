namespace Academic.Application.DTOs;

public class RecordGradeRequest
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public Guid RegistrationId { get; set; }
    public string Semester { get; set; } = string.Empty;
    public float MidtermScore { get; set; }
    public float FinalScore { get; set; }
    public int ECTS { get; set; }
}