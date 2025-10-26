namespace Academic.Application.DTOs;

public class GradeObjectionResponse
{
    public Guid Id { get; set; }
    public Guid GradeId { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public DateTime ObjectionDate { get; set; }
    public DateTime ObjectionDeadline { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int AppealLevel { get; set; }
    public Guid? ReviewedBy { get; set; }
    public DateTime? ReviewedDate { get; set; }
    public string? ReviewNotes { get; set; }
    public float? NewScore { get; set; }
    public string? NewLetterGrade { get; set; }
}