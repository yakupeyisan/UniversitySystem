namespace Academic.Application.DTOs;

public class PostponeExamRequest
{
    public Guid ExamId { get; set; }
    public string NewExamDate { get; set; } = string.Empty;
    public string NewStartTime { get; set; } = string.Empty;
    public string NewEndTime { get; set; } = string.Empty;
}