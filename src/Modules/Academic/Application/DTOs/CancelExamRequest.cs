namespace Academic.Application.DTOs;

public class CancelExamRequest
{
    public Guid ExamId { get; set; }
    public string Reason { get; set; } = string.Empty;
}