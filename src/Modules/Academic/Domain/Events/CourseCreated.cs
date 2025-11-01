using Academic.Domain.Enums;
using Core.Domain.Events;
namespace Academic.Domain.Events;
public class CourseCreated : DomainEvent
{
    public CourseCreated(Guid courseId, string courseCode, string courseName, CourseSemester semester)
    {
        CourseId = courseId;
        CourseCode = courseCode;
        CourseName = courseName;
        Semester = semester;
    }
    public Guid CourseId { get; }
    public string CourseCode { get; }
    public string CourseName { get; }
    public CourseSemester Semester { get; }
    public Guid AggregateId => CourseId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}