using Academic.Domain.Aggregates;
using Core.Domain.Specifications;
namespace Academic.Domain.Specifications;
public class ExamByCourseSpec : Specification<Exam>
{
    public ExamByCourseSpec(Guid courseId)
    {
        Criteria = e => e.CourseId == courseId && !e.IsDeleted;
        AddOrderBy(e => e.ExamDate);
    }
}