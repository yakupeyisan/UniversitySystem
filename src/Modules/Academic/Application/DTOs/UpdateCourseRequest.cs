namespace Academic.Application.DTOs;

public class UpdateCourseRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ECTS { get; set; }
    public int Credits { get; set; }
    public int MaxCapacity { get; set; }
}