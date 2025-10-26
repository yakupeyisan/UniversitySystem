namespace Academic.Application.DTOs;

public class StudentCoursesResponse
{
    public Guid StudentId { get; set; }
    public List<CourseRegistrationResponse> Courses { get; set; } = new();
    public int TotalEnrolledCourses { get; set; }
    public int TotalECTS { get; set; }
}