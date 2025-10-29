using Academic.Domain.Aggregates;
using Academic.Domain.Interfaces;
using Academic.Domain.Specifications;
using Shared.Infrastructure.Persistence.Contexts;
namespace Shared.Infrastructure.Persistence.Repositories.Academic;
public class PrerequisiteWaiverRepository : GenericRepository<PrerequisiteWaiver>, IPrerequisiteWaiverRepository
{
    public PrerequisiteWaiverRepository(AppDbContext context) : base(context) { }
    public async Task<IEnumerable<PrerequisiteWaiver>> GetByStudentAsync(Guid studentId, CancellationToken ct = default)
    {
        var spec = new PrerequisiteWaiversByStudentSpec(studentId);
        var result = await GetAllAsync(spec, ct);
        return result;
    }
    public async Task<IEnumerable<PrerequisiteWaiver>> GetPendingWaiversAsync(CancellationToken ct = default)
    {
        var spec = new PrerequisiteWaiversPendingSpec();
        var result = await GetAllAsync(spec, ct);
        return result;
    }
    public async Task<PrerequisiteWaiver?> GetByStudentAndPrerequisiteAsync(Guid studentId, Guid prerequisiteId, CancellationToken ct = default)
    {
        var spec = new PrerequisiteWaiverByStudentAndPrerequisiteSpec(studentId, prerequisiteId);
        return await GetAsync(spec, ct);
    }
    public async Task<IEnumerable<PrerequisiteWaiver>> GetApprovedWaiversAsync(Guid studentId, CancellationToken ct = default)
    {
        var spec = new ApprovedPrerequisiteWaiversSpec(studentId);
        var result = await GetAllAsync(spec, ct);
        return result;
    }
}