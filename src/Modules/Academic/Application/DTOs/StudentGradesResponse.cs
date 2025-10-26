namespace Academic.Application.DTOs;

public class StudentGradesResponse
{
    public Guid StudentId { get; set; }
    public List<GradeResponse> Grades { get; set; } = new();
    public float CumulativeGPA { get; set; }
    public float TotalECTS { get; set; }
}