using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting grade objections by course
/// </summary>
public class GradeObjectionsByCourseSpec : Specification<GradeObjection>
{
    public GradeObjectionsByCourseSpec(Guid courseId)
    {
        Criteria = go => go.CourseId == courseId && !go.IsDeleted;
        AddOrderByDescending(go => go.ObjectionDate);
    }
}