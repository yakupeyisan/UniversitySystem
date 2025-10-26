using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting exams that are not cancelled
/// Used for conflict checking
/// </summary>
public class NotCancelledExamsSpec : Specification<Exam>
{
    public NotCancelledExamsSpec()
    {
        Criteria = e => !e.IsDeleted && e.Status != ExamStatus.Cancelled;
        AddOrderBy(e => e.ExamDate);
    }
}