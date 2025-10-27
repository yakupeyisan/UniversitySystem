namespace Academic.Application.DTOs;
public class RemoveInstructorRequest
{
    public Guid CourseId { get; set; }
    public Guid InstructorId { get; set; }
}