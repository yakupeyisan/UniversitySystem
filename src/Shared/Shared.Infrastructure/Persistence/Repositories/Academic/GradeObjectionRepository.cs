using Academic.Domain.Aggregates;
using Academic.Domain.Specifications;
using Shared.Infrastructure.Persistence.Contexts;

namespace Shared.Infrastructure.Persistence.Repositories.Academic;

public class GradeObjectionRepository : GenericRepository<GradeObjection>, IGradeObjectionRepository
{
    public GradeObjectionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<GradeObjection>> GetByStudentAsync(Guid studentId, CancellationToken ct = default)
    {
        var spec = new GradeObjectionsByStudentSpec(studentId);
        var result = await GetAllAsync(spec, ct);
        return result;
    }

    public async Task<IEnumerable<GradeObjection>> GetByGradeAsync(Guid gradeId, CancellationToken ct = default)
    {
        var spec = new GradeObjectionByGradeSpec(gradeId);
        var result = await GetAllAsync(spec, ct);
        return result;
    }

    public async Task<IEnumerable<GradeObjection>> GetPendingObjectionsAsync(CancellationToken ct = default)
    {
        var spec = new PendingGradeObjectionsSpec();
        var result = await GetAllAsync(spec, ct);
        return result;
    }

    public async Task<GradeObjection?> GetByStudentAndGradeAsync(Guid studentId, Guid gradeId,
        CancellationToken ct = default)
    {
        var spec = new GradeObjectionByStudentAndGradeSpec(studentId, gradeId);
        return await GetAsync(spec, ct);
    }
}