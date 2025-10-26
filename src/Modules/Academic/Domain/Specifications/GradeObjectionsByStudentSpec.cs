using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting grade objections by student
/// </summary>
public class GradeObjectionsByStudentSpec : Specification<GradeObjection>
{
    public GradeObjectionsByStudentSpec(Guid studentId)
    {
        Criteria = go => go.StudentId == studentId && !go.IsDeleted;
        AddInclude(go => go.Grade);
        AddOrderByDescending(go => go.ObjectionDate);
    }
}