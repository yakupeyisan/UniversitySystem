using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

public class PrerequisiteWaiverByStudentAndPrerequisiteSpec : Specification<PrerequisiteWaiver>
{
    public PrerequisiteWaiverByStudentAndPrerequisiteSpec(Guid studentId, Guid prerequisiteId)
    {
        Criteria = pw => pw.StudentId == studentId && pw.PrerequisiteId == prerequisiteId && !pw.IsDeleted;
    }
}