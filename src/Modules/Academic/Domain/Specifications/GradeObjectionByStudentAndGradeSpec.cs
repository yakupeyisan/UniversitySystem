using Academic.Domain.Aggregates;
using Core.Domain.Specifications;
namespace Academic.Domain.Specifications;
public class GradeObjectionByStudentAndGradeSpec : Specification<GradeObjection>
{
    public GradeObjectionByStudentAndGradeSpec(Guid studentId, Guid gradeId)
    {
        Criteria = go => go.StudentId == studentId && go.GradeId == gradeId && !go.IsDeleted;
    }
}