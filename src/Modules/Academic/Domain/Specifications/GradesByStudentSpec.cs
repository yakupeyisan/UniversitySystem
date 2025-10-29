using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

public class GradesByStudentSpec : Specification<Grade>
{
    public GradesByStudentSpec(Guid studentId)
    {
        Criteria = g => g.StudentId == studentId && !g.IsDeleted;
        AddInclude(g => g.Course);
        AddOrderByDescending(g => g.CreatedAt);
    }
}