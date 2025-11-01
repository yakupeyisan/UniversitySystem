namespace Academic.Application.DTOs;
public class ApproveGradeObjectionRequest
{
    public Guid ObjectionId { get; set; }
    public Guid ReviewedBy { get; set; }
    public string? ReviewNotes { get; set; }
    public float? NewScore { get; set; }
    public int? NewLetterGrade { get; set; }
}