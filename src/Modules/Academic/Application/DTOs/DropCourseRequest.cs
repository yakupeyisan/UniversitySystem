namespace Academic.Application.DTOs;

public class DropCourseRequest
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string Reason { get; set; } = string.Empty;
}