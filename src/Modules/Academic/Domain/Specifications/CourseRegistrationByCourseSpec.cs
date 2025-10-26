using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting course registrations by course
/// </summary>
public class CourseRegistrationByCourseSpec : Specification<CourseRegistration>
{
    public CourseRegistrationByCourseSpec(Guid courseId)
    {
        Criteria = cr => cr.CourseId == courseId && !cr.IsDeleted;
        AddOrderByDescending(cr => cr.CreatedAt);
    }
}