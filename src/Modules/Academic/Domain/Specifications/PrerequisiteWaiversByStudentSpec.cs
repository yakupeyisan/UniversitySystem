using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting prerequisite waivers by student
/// </summary>
public class PrerequisiteWaiversByStudentSpec : Specification<PrerequisiteWaiver>
{
    public PrerequisiteWaiversByStudentSpec(Guid studentId)
    {
        Criteria = pw => pw.StudentId == studentId && !pw.IsDeleted;
        AddInclude(pw => pw.Prerequisite);
        AddOrderByDescending(pw => pw.RequestedDate);
    }
}