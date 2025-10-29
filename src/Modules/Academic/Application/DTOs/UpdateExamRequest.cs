namespace Academic.Application.DTOs;

public class UpdateExamRequest
{
    public Guid ExamId { get; set; }
    public string ExamDate { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public Guid? ExamRoomId { get; set; }
}