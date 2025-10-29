using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

public class PrerequisiteWaiversPendingSpec : Specification<PrerequisiteWaiver>
{
    public PrerequisiteWaiversPendingSpec()
    {
        Criteria = pw => pw.Status == PrerequisiteWaiverStatus.Pending && !pw.IsDeleted;
        AddOrderBy(pw => pw.RequestedDate);
    }
}