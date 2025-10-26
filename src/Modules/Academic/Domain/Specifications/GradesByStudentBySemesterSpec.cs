using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting grades by student for a specific semester
/// NOTE: Ordering by CreatedAt instead of Course.CourseCode (nested property)
/// </summary>
public class GradesByStudentBySemesterSpec : Specification<Grade>
{
    public GradesByStudentBySemesterSpec(Guid studentId, string semester)
    {
        Criteria = g => g.StudentId == studentId &&
                        g.Semester == semester &&
                        !g.IsDeleted;
        AddInclude(g => g.Course);
        AddOrderBy(g => g.Semester);
    }
}