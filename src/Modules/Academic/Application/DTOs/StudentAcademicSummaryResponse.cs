namespace Academic.Application.DTOs;
public class StudentAcademicSummaryResponse
{
    public Guid StudentId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public float CumulativeGPA { get; set; }
    public int CompletedECTS { get; set; }
    public int EnrolledECTS { get; set; }
    public int CompletedCourses { get; set; }
    public int EnrolledCourses { get; set; }
    public int FailedCourses { get; set; }
    public string AcademicStanding { get; set; } = string.Empty;
    public DateTime? LastAcademicEvaluationDate { get; set; }
    public List<GradeResponse> RecentGrades { get; set; } = new();
    public List<CourseRegistrationResponse> CurrentCourses { get; set; } = new();
}