using Academic.Domain.Enums;
using Core.Domain.Events;

namespace Academic.Domain.Events;

/// <summary>
/// Event raised when a grade objection is approved
/// </summary>
public class GradeObjectionApproved : DomainEvent
{
    public Guid ObjectionId { get; }
    public Guid GradeId { get; }
    public Guid StudentId { get; }
    public float? NewScore { get; }
    public LetterGrade? NewLetterGrade { get; }
    public Guid AggregateId => ObjectionId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public GradeObjectionApproved(Guid objectionId, Guid gradeId, Guid studentId, float? newScore, LetterGrade? newLetterGrade)
    {
        ObjectionId = objectionId;
        GradeId = gradeId;
        StudentId = studentId;
        NewScore = newScore;
        NewLetterGrade = newLetterGrade;
    }
}