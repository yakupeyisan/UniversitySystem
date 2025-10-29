using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

public class WaitingListAdmittedEntriesByStatusSpec : Specification<CourseWaitingListEntry>
{
    public WaitingListAdmittedEntriesByStatusSpec(Guid courseId)
    {
        Criteria = wl => wl.CourseId == courseId && wl.Status == WaitingListStatus.Admitted && !wl.IsDeleted;
        AddOrderBy(wl => wl.QueuePosition);
    }
}