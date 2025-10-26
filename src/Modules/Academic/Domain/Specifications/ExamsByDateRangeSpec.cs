using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting exams by date range
/// NOTE: Only ordering by ExamDate (scalar property), not by TimeSlot.StartTime (ValueObject)
/// </summary>
public class ExamsByDateRangeSpec : Specification<Exam>
{
    public ExamsByDateRangeSpec(DateOnly startDate, DateOnly endDate)
    {
        Criteria = e => e.ExamDate >= startDate &&
                        e.ExamDate <= endDate &&
                        e.Status != ExamStatus.Cancelled &&
                        !e.IsDeleted;
        AddOrderBy(e => e.ExamDate);
    }
}