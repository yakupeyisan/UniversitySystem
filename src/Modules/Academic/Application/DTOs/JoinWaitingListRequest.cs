namespace Academic.Application.DTOs;

public class JoinWaitingListRequest
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string Semester { get; set; } = string.Empty;
}