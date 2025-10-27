namespace Academic.Application.DTOs;
public class CourseExamsResponse
{
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public List<ExamResponse> Exams { get; set; } = new();
}