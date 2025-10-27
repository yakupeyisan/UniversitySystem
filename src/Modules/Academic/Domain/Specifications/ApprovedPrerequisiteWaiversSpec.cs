using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using Core.Domain.Specifications;
namespace Academic.Domain.Specifications;
public class ApprovedPrerequisiteWaiversSpec : Specification<PrerequisiteWaiver>
{
    public ApprovedPrerequisiteWaiversSpec(Guid studentId)
    {
        var now = DateTime.UtcNow;
        Criteria = pw => pw.StudentId == studentId &&
                         pw.Status == PrerequisiteWaiverStatus.Approved &&
                         pw.ExpiryDate >= now &&
                         !pw.IsDeleted;
        AddOrderByDescending(pw => pw.ApprovedDate);
    }
}