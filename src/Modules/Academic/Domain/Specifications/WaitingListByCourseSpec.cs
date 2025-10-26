using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting waiting list entries by course, ordered by queue position
/// </summary>
public class WaitingListByCourseSpec : Specification<CourseWaitingListEntry>
{
    public WaitingListByCourseSpec(Guid courseId)
    {
        Criteria = wl => wl.CourseId == courseId &&
                         wl.Status == WaitingListStatus.Waiting &&
                         !wl.IsDeleted;
        AddOrderBy(wl => wl.QueuePosition);
    }
}