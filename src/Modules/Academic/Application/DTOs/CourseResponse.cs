namespace Academic.Application.DTOs;
public class CourseResponse
{
    public Guid Id { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ECTS { get; set; }
    public int Credits { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Semester { get; set; } = string.Empty;
    public int Year { get; set; }
    public Guid DepartmentId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public int CurrentEnrollment { get; set; }
    public float OccupancyPercentage { get; set; }
    public int InstructorCount { get; set; }
    public int PrerequisiteCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}