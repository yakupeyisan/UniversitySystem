namespace Academic.Application.DTOs;

public class CancelCourseRequest
{
    public Guid CourseId { get; set; }
    public string Reason { get; set; } = string.Empty;
}