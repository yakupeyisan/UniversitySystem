using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

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