using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

public class CourseRegistrationBySemesterSpec : Specification<CourseRegistration>
{
    public CourseRegistrationBySemesterSpec(Guid studentId, string semester)
    {
        Criteria = cr => cr.StudentId == studentId && cr.Semester == semester && !cr.IsDeleted;
        AddInclude(cr => cr.Course);
        AddOrderBy(cr => cr.CreatedAt);
    }
}