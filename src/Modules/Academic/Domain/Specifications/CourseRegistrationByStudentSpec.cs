using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting course registrations by student
/// </summary>
public class CourseRegistrationByStudentSpec : Specification<CourseRegistration>
{
    public CourseRegistrationByStudentSpec(Guid studentId)
    {
        Criteria = cr => cr.StudentId == studentId && !cr.IsDeleted;
        AddInclude(cr => cr.Course);
        AddOrderByDescending(cr => cr.CreatedAt);
    }
}