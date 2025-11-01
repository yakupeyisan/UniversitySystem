namespace Academic.Application.DTOs;
public class CourseStatisticsResponse
{
    public Guid CourseId { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public int TotalEnrollments { get; set; }
    public int MaxCapacity { get; set; }
    public int WaitingListCount { get; set; }
    public double EnrollmentPercentage { get; set; }
    public double AverageScore { get; set; }
    public double PassRate { get; set; }
    public int PassedCount { get; set; }
    public int FailedCount { get; set; }
    public double AverageGPA { get; set; }
}