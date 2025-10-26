using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting prerequisite waiver by student and prerequisite
/// </summary>
public class PrerequisiteWaiverByStudentAndPrerequisiteSpec : Specification<PrerequisiteWaiver>
{
    public PrerequisiteWaiverByStudentAndPrerequisiteSpec(Guid studentId, Guid prerequisiteId)
    {
        Criteria = pw => pw.StudentId == studentId && pw.PrerequisiteId == prerequisiteId && !pw.IsDeleted;
    }
}