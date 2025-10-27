using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using Core.Domain.Specifications;
namespace Academic.Domain.Specifications;
public class PendingWaitingListSpec : Specification<CourseWaitingListEntry>
{
    public PendingWaitingListSpec()
    {
        Criteria = wl => wl.Status == WaitingListStatus.Waiting && !wl.IsDeleted;
        AddOrderBy(wl => wl.RequestedDate);
    }
}