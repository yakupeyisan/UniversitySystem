using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

public class GradeObjectionsByStudentSpec : Specification<GradeObjection>
{
    public GradeObjectionsByStudentSpec(Guid studentId)
    {
        Criteria = go => go.StudentId == studentId && !go.IsDeleted;
        AddInclude(go => go.Grade);
        AddOrderByDescending(go => go.ObjectionDate);
    }
}