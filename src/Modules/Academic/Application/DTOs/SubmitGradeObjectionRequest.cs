namespace Academic.Application.DTOs;
public class SubmitGradeObjectionRequest
{
    public Guid GradeId { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string Reason { get; set; } = string.Empty;
}