using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting waiting list entries by student
/// </summary>
public class WaitingListByStudentSpec : Specification<CourseWaitingListEntry>
{
    public WaitingListByStudentSpec(Guid studentId)
    {
        Criteria = wl => wl.StudentId == studentId && !wl.IsDeleted;
        AddInclude(wl => wl.Course);
        AddOrderBy(wl => wl.RequestedDate);
    }
}