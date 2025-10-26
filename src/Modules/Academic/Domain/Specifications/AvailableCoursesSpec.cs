using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting available courses (not deleted, status is active)
/// </summary>
public class AvailableCoursesSpec : Specification<Course>
{
    public AvailableCoursesSpec()
    {
        Criteria = c => !c.IsDeleted && c.Status == CourseStatus.Active;
        AddOrderBy(c => c.CourseCode);
    }
}