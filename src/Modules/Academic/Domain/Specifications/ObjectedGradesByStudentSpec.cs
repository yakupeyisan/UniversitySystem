using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting objected grades by student
/// </summary>
public class ObjectedGradesByStudentSpec : Specification<Grade>
{
    public ObjectedGradesByStudentSpec(Guid studentId)
    {
        Criteria = g => g.StudentId == studentId && g.IsObjected && !g.IsDeleted;
    }
}