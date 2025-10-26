namespace Academic.Application.DTOs;

public class RejectGradeObjectionRequest
{
    public Guid ObjectionId { get; set; }
    public Guid ReviewedBy { get; set; }
    public string? ReviewNotes { get; set; }
}