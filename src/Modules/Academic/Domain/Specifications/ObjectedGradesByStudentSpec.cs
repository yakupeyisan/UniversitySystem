using Academic.Domain.Aggregates;
using Core.Domain.Specifications;
namespace Academic.Domain.Specifications;
public class ObjectedGradesByStudentSpec : Specification<Grade>
{
    public ObjectedGradesByStudentSpec(Guid studentId)
    {
        Criteria = g => g.StudentId == studentId && g.IsObjected && !g.IsDeleted;
    }
}