namespace Academic.Application.DTOs;
public class ScheduleExamRequest
{
    public Guid CourseId { get; set; }
    public int ExamType { get; set; }
    public string ExamDate { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public Guid? ExamRoomId { get; set; }
    public bool IsOnline { get; set; }
    public string? OnlineLink { get; set; }
}