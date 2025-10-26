using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting pending grade objections
/// </summary>
public class PendingGradeObjectionsSpec : Specification<GradeObjection>
{
    public PendingGradeObjectionsSpec()
    {
        Criteria = go => go.Status == GradeObjectionStatus.Pending && !go.IsDeleted;
        AddInclude(go => go.Grade);
        AddOrderBy(go => go.ObjectionDate);
    }
}