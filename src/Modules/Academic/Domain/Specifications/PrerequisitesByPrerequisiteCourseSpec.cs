using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting prerequisites by prerequisite course
/// </summary>
public class PrerequisitesByPrerequisiteCourseSpec : Specification<Prerequisite>
{
    public PrerequisitesByPrerequisiteCourseSpec(Guid prerequisiteCourseId)
    {
        Criteria = p => p.PrerequisiteCourseId == prerequisiteCourseId;
    }
}