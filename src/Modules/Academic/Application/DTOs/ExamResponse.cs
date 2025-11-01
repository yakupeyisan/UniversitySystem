namespace Academic.Application.DTOs;
public class ExamResponse
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string ExamType { get; set; } = string.Empty;
    public string ExamDate { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public Guid? ExamRoomId { get; set; }
    public int MaxCapacity { get; set; }
    public int CurrentRegisteredCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
    public string? OnlineLink { get; set; }
}