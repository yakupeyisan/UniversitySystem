using Academic.Domain.Aggregates;
using Core.Domain.Specifications;
namespace Academic.Domain.Specifications;
public class CourseByDepartmentSpec : Specification<Course>
{
    public CourseByDepartmentSpec(Guid departmentId)
    {
        Criteria = c => c.DepartmentId == departmentId && !c.IsDeleted;
        AddOrderBy(c => c.Code.Value);
    }
}