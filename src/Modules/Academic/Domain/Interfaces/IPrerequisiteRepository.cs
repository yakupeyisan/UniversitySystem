using Academic.Domain.Aggregates;
using Core.Domain.Repositories;
namespace Academic.Domain.Interfaces;
public interface IPrerequisiteRepository : IRepository<Prerequisite>
{
    Task<IEnumerable<Prerequisite>> GetByPrerequisiteCourseAsync(Guid prerequisiteCourseId, CancellationToken ct = default);
    Task<Prerequisite?> GetByCoursesAsync(Guid courseId, Guid prerequisiteCourseId, CancellationToken ct = default);
}