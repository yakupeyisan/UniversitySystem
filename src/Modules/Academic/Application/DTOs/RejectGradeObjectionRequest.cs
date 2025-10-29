namespace Academic.Application.DTOs;

public class RejectGradeObjectionRequest
{
    public Guid ReviewedBy { get; set; }
    public string RejectionReason { get; set; } = string.Empty;
    public string? AdditionalNotes { get; set; }
}