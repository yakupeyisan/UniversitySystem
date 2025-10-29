using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

public class WaitingListByStudentAndCourseSpec : Specification<CourseWaitingListEntry>
{
    public WaitingListByStudentAndCourseSpec(Guid studentId, Guid courseId)
    {
        Criteria = wl => wl.StudentId == studentId && wl.CourseId == courseId && !wl.IsDeleted;
    }
}