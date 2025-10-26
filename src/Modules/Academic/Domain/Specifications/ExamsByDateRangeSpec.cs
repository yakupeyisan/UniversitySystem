using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting exams by date range
/// </summary>
public class ExamsByDateRangeSpec : Specification<Exam>
{
    public ExamsByDateRangeSpec(DateOnly startDate, DateOnly endDate)
    {
        Criteria = e => e.ExamDate >= startDate && e.ExamDate <= endDate && !e.IsDeleted;
        AddOrderBy(e => e.ExamDate);
    }
}