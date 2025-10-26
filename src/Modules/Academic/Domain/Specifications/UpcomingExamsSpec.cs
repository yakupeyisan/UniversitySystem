using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting upcoming exams
/// NOTE: Only ordering by ExamDate (scalar property), not by TimeSlot.StartTime (ValueObject)
/// </summary>
public class UpcomingExamsSpec : Specification<Exam>
{
    public UpcomingExamsSpec()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        Criteria = e => e.ExamDate >= today &&
                        e.Status != ExamStatus.Cancelled &&
                        !e.IsDeleted;
        AddOrderBy(e => e.ExamDate);
    }
}