using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting course registrations for a specific semester
/// NOTE: Ordering by Semester instead of Course.CourseCode (nested property)
/// </summary>
public class CourseRegistrationBySemesterSpec : Specification<CourseRegistration>
{
    public CourseRegistrationBySemesterSpec(Guid studentId, string semester)
    {
        Criteria = cr => cr.StudentId == studentId && cr.Semester == semester && !cr.IsDeleted;
        AddInclude(cr => cr.Course);
        AddOrderBy(cr => cr.CreatedAt);
    }
}