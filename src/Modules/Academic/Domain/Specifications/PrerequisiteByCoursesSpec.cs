using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting prerequisite by courses
/// </summary>
public class PrerequisiteByCoursesSpec : Specification<Prerequisite>
{
    public PrerequisiteByCoursesSpec(Guid courseId, Guid prerequisiteCourseId)
    {
        Criteria = p => p.CourseId == courseId && p.PrerequisiteCourseId == prerequisiteCourseId;
    }
}