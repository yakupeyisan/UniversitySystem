using Academic.Domain.Aggregates;
using Core.Domain.Repositories;

namespace Academic.Domain.Interfaces;

/// <summary>
/// Repository interface for PrerequisiteWaiver aggregate
/// </summary>
public interface IPrerequisiteWaiverRepository : IRepository<PrerequisiteWaiver>
{
    Task<IEnumerable<PrerequisiteWaiver>> GetByStudentAsync(Guid studentId, CancellationToken ct = default);
    Task<IEnumerable<PrerequisiteWaiver>> GetPendingWaiversAsync(CancellationToken ct = default);
    Task<PrerequisiteWaiver?> GetByStudentAndPrerequisiteAsync(Guid studentId, Guid prerequisiteId, CancellationToken ct = default);
    Task<IEnumerable<PrerequisiteWaiver>> GetApprovedWaiversAsync(Guid studentId, CancellationToken ct = default);
}