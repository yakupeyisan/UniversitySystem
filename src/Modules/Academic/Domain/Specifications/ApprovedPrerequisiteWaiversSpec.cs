using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting approved prerequisite waivers that are not expired
/// </summary>
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