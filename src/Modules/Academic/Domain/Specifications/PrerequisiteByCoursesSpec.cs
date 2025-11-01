using Academic.Domain.Aggregates;
using Core.Domain.Specifications;
namespace Academic.Domain.Specifications;
public class PrerequisiteByCoursesSpec : Specification<Prerequisite>
{
    public PrerequisiteByCoursesSpec(Guid courseId, Guid prerequisiteCourseId)
    {
        Criteria = p => p.CourseId == courseId && p.PrerequisiteCourseId == prerequisiteCourseId;
    }
}