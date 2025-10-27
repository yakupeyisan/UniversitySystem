using Academic.Domain.Enums;
using Academic.Domain.ValueObjects;
using Core.Domain.Events;
namespace Academic.Domain.Events;
public class ExamScheduled : DomainEvent
{
    public Guid ExamId { get; }
    public Guid CourseId { get; }
    public ExamType ExamType { get; }
    public DateOnly ExamDate { get; }
    public TimeSlot TimeSlot { get; }
    public Guid AggregateId => ExamId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public ExamScheduled(Guid examId, Guid courseId, ExamType examType, DateOnly examDate, TimeSlot timeSlot)
    {
        ExamId = examId;
        CourseId = courseId;
        ExamType = examType;
        ExamDate = examDate;
        TimeSlot = timeSlot;
    }
}