using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

public class GradeByStudentAndCourseSpec : Specification<Grade>
{
    public GradeByStudentAndCourseSpec(Guid studentId, Guid courseId)
    {
        Criteria = g => g.StudentId == studentId && g.CourseId == courseId && !g.IsDeleted;
    }
}