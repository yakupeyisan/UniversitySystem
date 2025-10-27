using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using Core.Domain.Specifications;
namespace Academic.Domain.Specifications;
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