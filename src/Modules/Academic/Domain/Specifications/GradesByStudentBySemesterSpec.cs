using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

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