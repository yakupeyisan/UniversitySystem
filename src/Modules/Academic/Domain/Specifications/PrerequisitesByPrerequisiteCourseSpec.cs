using Academic.Domain.Aggregates;
using Core.Domain.Specifications;
namespace Academic.Domain.Specifications;
public class PrerequisitesByPrerequisiteCourseSpec : Specification<Prerequisite>
{
    public PrerequisitesByPrerequisiteCourseSpec(Guid prerequisiteCourseId)
    {
        Criteria = p => p.PrerequisiteCourseId == prerequisiteCourseId;
    }
}