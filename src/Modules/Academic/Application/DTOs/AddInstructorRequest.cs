namespace Academic.Application.DTOs;

public class AddInstructorRequest
{
    public Guid CourseId { get; set; }
    public Guid InstructorId { get; set; }
}