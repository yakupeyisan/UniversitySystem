using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using Core.Domain.Specifications;
namespace Academic.Domain.Specifications;
public class NotCancelledExamsSpec : Specification<Exam>
{
    public NotCancelledExamsSpec()
    {
        Criteria = e => !e.IsDeleted && e.Status != ExamStatus.Cancelled;
        AddOrderBy(e => e.ExamDate);
    }
}