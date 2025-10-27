using Academic.Domain.Aggregates;
using Core.Domain.Repositories;
namespace Academic.Domain.Interfaces;
public interface ICourseRepository : IRepository<Course>
{
    Task<Course?> GetByCourseCodeAsync(string courseCode, CancellationToken ct = default);
    Task<IEnumerable<Course>> GetByLevelAsync(int level, CancellationToken ct = default);
    Task<IEnumerable<Course>> GetAvailableCoursesAsync(CancellationToken ct = default);
    Task<IEnumerable<Course>> GetByDepartmentAsync(Guid departmentId, CancellationToken ct = default);
}