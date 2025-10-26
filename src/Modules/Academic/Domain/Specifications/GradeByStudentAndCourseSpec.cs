using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting grade by student and course
/// </summary>
public class GradeByStudentAndCourseSpec : Specification<Grade>
{
    public GradeByStudentAndCourseSpec(Guid studentId, Guid courseId)
    {
        Criteria = g => g.StudentId == studentId && g.CourseId == courseId && !g.IsDeleted;
    }
}