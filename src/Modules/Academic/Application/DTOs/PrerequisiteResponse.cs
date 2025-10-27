namespace Academic.Application.DTOs;
public class PrerequisiteResponse
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid PrerequisiteCourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string PrerequisiteCourseName { get; set; } = string.Empty;
    public double MinimumGradePoint { get; set; }
}