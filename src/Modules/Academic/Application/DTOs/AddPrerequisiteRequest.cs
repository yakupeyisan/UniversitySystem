namespace Academic.Application.DTOs;

public class AddPrerequisiteRequest
{
    public Guid CourseId { get; set; }
    public Guid PrerequisiteCourseId { get; set; }
}