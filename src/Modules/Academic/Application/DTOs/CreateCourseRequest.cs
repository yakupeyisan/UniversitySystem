namespace Academic.Application.DTOs;

public class CreateCourseRequest
{
    public string CourseCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ECTS { get; set; }
    public int Credits { get; set; }
    public int Level { get; set; }
    public int Type { get; set; }
    public int Semester { get; set; }
    public int Year { get; set; }
    public Guid DepartmentId { get; set; }
    public int MaxCapacity { get; set; }
}