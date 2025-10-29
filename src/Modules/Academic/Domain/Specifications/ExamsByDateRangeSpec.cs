using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

public class ExamsByDateRangeSpec : Specification<Exam>
{
    public ExamsByDateRangeSpec(DateOnly startDate, DateOnly endDate)
    {
        Criteria = e => e.ExamDate >= startDate && e.ExamDate <= endDate && !e.IsDeleted;
        AddOrderBy(e => e.ExamDate);
    }
}