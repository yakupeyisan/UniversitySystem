using Academic.Domain.Enums;
using Core.Domain.Events;

namespace Academic.Domain.Events;

/// <summary>
/// Event raised when a grade is recorded
/// </summary>
public class GradeRecorded : DomainEvent
{
    public Guid GradeId { get; }
    public Guid StudentId { get; }
    public Guid CourseId { get; }
    public LetterGrade LetterGrade { get; }
    public float NumericScore { get; }
    public Guid AggregateId => GradeId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public GradeRecorded(Guid gradeId, Guid studentId, Guid courseId, LetterGrade letterGrade, float numericScore)
    {
        GradeId = gradeId;
        StudentId = studentId;
        CourseId = courseId;
        LetterGrade = letterGrade;
        NumericScore = numericScore;
    }
}