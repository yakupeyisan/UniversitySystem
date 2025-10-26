using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting exams by course
/// NOTE: Only ordering by ExamDate (scalar property), not by TimeSlot.StartTime (ValueObject)
/// </summary>
public class ExamByCourseSpec : Specification<Exam>
{
    public ExamByCourseSpec(Guid courseId)
    {
        Criteria = e => e.CourseId == courseId && !e.IsDeleted;
        AddOrderBy(e => e.ExamDate);
    }
}