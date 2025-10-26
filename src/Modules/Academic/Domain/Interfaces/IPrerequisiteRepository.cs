using Academic.Domain.Aggregates;
using Core.Domain.Repositories;

namespace Academic.Domain.Interfaces;

/// <summary>
/// Repository interface for Prerequisite entity
/// </summary>
public interface IPrerequisiteRepository : IRepository<Prerequisite>
{
    Task<IEnumerable<Prerequisite>> GetByPrerequisiteCourseAsync(Guid prerequisiteCourseId, CancellationToken ct = default);
    Task<Prerequisite?> GetByCoursesAsync(Guid courseId, Guid prerequisiteCourseId, CancellationToken ct = default);
}