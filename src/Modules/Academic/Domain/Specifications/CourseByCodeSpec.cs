using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

public class CourseByCodeSpec : Specification<Course>
{
    public CourseByCodeSpec(string courseCode)
    {
        Criteria = c => c.Code.Value == courseCode && !c.IsDeleted;
    }
}