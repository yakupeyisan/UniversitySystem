namespace Academic.Application.DTOs;

public class ScheduleExamRequest
{
    public Guid CourseId { get; set; }
    public int ExamType { get; set; }
    public string ExamDate { get; set; } = string.Empty; // Date format: yyyy-MM-dd
    public string StartTime { get; set; } = string.Empty; // Time format: HH:mm
    public string EndTime { get; set; } = string.Empty;   // Time format: HH:mm
    public int MaxCapacity { get; set; }
    public Guid? ExamRoomId { get; set; }
    public bool IsOnline { get; set; }
    public string? OnlineLink { get; set; }
}