using Academic.Domain.Enums;
using Core.Domain.Events;
namespace Academic.Domain.Events;
public class GradeObjectionApproved : DomainEvent
{
    public GradeObjectionApproved(Guid objectionId, Guid gradeId, Guid studentId, float? newScore,
        LetterGrade? newLetterGrade)
    {
        ObjectionId = objectionId;
        GradeId = gradeId;
        StudentId = studentId;
        NewScore = newScore;
        NewLetterGrade = newLetterGrade;
    }
    public Guid ObjectionId { get; }
    public Guid GradeId { get; }
    public Guid StudentId { get; }
    public float? NewScore { get; }
    public LetterGrade? NewLetterGrade { get; }
    public Guid AggregateId => ObjectionId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}