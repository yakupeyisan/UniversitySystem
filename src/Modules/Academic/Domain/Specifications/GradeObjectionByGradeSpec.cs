using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

public class GradeObjectionByGradeSpec : Specification<GradeObjection>
{
    public GradeObjectionByGradeSpec(Guid gradeId)
    {
        Criteria = go => go.GradeId == gradeId && !go.IsDeleted;
    }
}