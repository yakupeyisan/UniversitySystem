using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

public class CoursesByLevelSpec : Specification<Course>
{
    public CoursesByLevelSpec(CourseLevel level)
    {
        Criteria = c => c.Level == level && !c.IsDeleted;
        AddOrderBy(c => c.Code.Value);
    }
}