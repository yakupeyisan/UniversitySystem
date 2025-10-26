namespace Academic.Application.DTOs;

public class CreateExamRequest
{
    public Guid CourseId { get; set; }
    public string ExamType { get; set; } = string.Empty; // Midterm, Final, etc.
    public DateOnly ExamDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public Guid ExamRoomId { get; set; }
    public string? Instructions { get; set; }
}