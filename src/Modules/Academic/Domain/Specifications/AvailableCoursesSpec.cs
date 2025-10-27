using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using Core.Domain.Specifications;
namespace Academic.Domain.Specifications;
public class AvailableCoursesSpec : Specification<Course>
{
    public AvailableCoursesSpec()
    {
        Criteria = c => !c.IsDeleted && c.Status == CourseStatus.Active;
        AddOrderBy(c => c.Code.Value);
    }
}