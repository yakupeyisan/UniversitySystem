using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting pending waiting list entries
/// </summary>
public class PendingWaitingListSpec : Specification<CourseWaitingListEntry>
{
    public PendingWaitingListSpec()
    {
        Criteria = wl => wl.Status == WaitingListStatus.Waiting && !wl.IsDeleted;
        AddOrderBy(wl => wl.RequestedDate);
    }
}