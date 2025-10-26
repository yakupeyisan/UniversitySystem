using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting grade objections by grade
/// </summary>
public class GradeObjectionByGradeSpec : Specification<GradeObjection>
{
    public GradeObjectionByGradeSpec(Guid gradeId)
    {
        Criteria = go => go.GradeId == gradeId && !go.IsDeleted;
    }
}