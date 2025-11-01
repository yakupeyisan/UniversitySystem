using Academic.Domain.Aggregates;
using Core.Domain.Specifications;
namespace Academic.Domain.Specifications;
public class ExamsByStudentSpec : Specification<Exam>
{
    public ExamsByStudentSpec(Guid studentId)
    {
        Criteria = e => !e.IsDeleted;
        AddInclude(e => e.Course);
        AddOrderBy(e => e.ExamDate);
    }
}