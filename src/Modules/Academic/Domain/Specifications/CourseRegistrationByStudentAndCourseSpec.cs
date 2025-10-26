using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting course registration by student and course
/// </summary>
public class CourseRegistrationByStudentAndCourseSpec : Specification<CourseRegistration>
{
    public CourseRegistrationByStudentAndCourseSpec(Guid studentId, Guid courseId)
    {
        Criteria = cr => cr.StudentId == studentId && cr.CourseId == courseId && !cr.IsDeleted;
    }
}