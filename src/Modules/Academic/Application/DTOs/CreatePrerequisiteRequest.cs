namespace Academic.Application.DTOs;

public class CreatePrerequisiteRequest
{
    public Guid CourseId { get; set; }
    public Guid PrerequisiteCourseId { get; set; }
    public double MinimumGradePoint { get; set; }
}