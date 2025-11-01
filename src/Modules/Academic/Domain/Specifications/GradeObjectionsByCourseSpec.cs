using Academic.Domain.Aggregates;
using Core.Domain.Specifications;
namespace Academic.Domain.Specifications;
public class GradeObjectionsByCourseSpec : Specification<GradeObjection>
{
    public GradeObjectionsByCourseSpec(Guid courseId)
    {
        Criteria = go => go.CourseId == courseId && !go.IsDeleted;
        AddOrderByDescending(go => go.ObjectionDate);
    }
}