using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

public class PrerequisiteWaiversByStudentSpec : Specification<PrerequisiteWaiver>
{
    public PrerequisiteWaiversByStudentSpec(Guid studentId)
    {
        Criteria = pw => pw.StudentId == studentId && !pw.IsDeleted;
        AddInclude(pw => pw.Prerequisite);
        AddOrderByDescending(pw => pw.RequestedDate);
    }
}