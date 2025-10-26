using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting exams
/// NOTE: Student filtering should be done in application layer via CourseRegistration join
/// </summary>
public class ExamsByStudentSpec : Specification<Exam>
{
    public ExamsByStudentSpec(Guid studentId)
    {
        // Basic specification - application layer handles student-exam relationship via CourseRegistration
        Criteria = e => !e.IsDeleted;
        AddInclude(e => e.Course);
        AddOrderBy(e => e.ExamDate);
    }
}