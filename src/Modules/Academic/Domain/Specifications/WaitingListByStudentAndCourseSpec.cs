using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting waiting list entry by student and course
/// </summary>
public class WaitingListByStudentAndCourseSpec : Specification<CourseWaitingListEntry>
{
    public WaitingListByStudentAndCourseSpec(Guid studentId, Guid courseId)
    {
        Criteria = wl => wl.StudentId == studentId && wl.CourseId == courseId && !wl.IsDeleted;
    }
}