namespace Academic.Application.DTOs;

public class GradeResponse
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string Semester { get; set; } = string.Empty;
    public float MidtermScore { get; set; }
    public float FinalScore { get; set; }
    public float NumericScore { get; set; }
    public string LetterGrade { get; set; } = string.Empty;
    public float GradePoint { get; set; }
    public int ECTS { get; set; }
    public bool IsObjected { get; set; }
    public DateTime ObjectionDeadline { get; set; }
    public DateTime RecordedDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}