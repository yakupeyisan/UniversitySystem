using Academic.Domain.Aggregates;
using Academic.Domain.Interfaces;
using Academic.Domain.Specifications;
using Shared.Infrastructure.Persistence.Contexts;

namespace Shared.Infrastructure.Persistence.Repositories.Academic;

/// <summary>
/// Repository for Prerequisite entity
/// </summary>
public class PrerequisiteRepository : GenericRepository<Prerequisite>, IPrerequisiteRepository
{
    public PrerequisiteRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Prerequisite>> GetByPrerequisiteCourseAsync(Guid prerequisiteCourseId, CancellationToken ct = default)
    {
        var spec = new PrerequisitesByPrerequisiteCourseSpec(prerequisiteCourseId);
        var result = await GetAllAsync(spec, ct);
        return result.Data;
    }

    public async Task<Prerequisite?> GetByCoursesAsync(Guid courseId, Guid prerequisiteCourseId, CancellationToken ct = default)
    {
        var spec = new PrerequisiteByCoursesSpec(courseId, prerequisiteCourseId);
        return await GetAsync(spec, ct);
    }
}