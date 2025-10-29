using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using Academic.Domain.Specifications;
using Shared.Infrastructure.Persistence.Contexts;

namespace Shared.Infrastructure.Persistence.Repositories.Academic;

public class CourseRepository : GenericRepository<Course>
{
    public CourseRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Course?> GetByCourseCodeAsync(string courseCode, CancellationToken ct = default)
    {
        var spec = new CourseByCodeSpec(courseCode);
        return await GetAsync(spec, ct);
    }

    public async Task<IEnumerable<Course>> GetByLevelAsync(int level, CancellationToken ct = default)
    {
        var spec = new CoursesByLevelSpec((CourseLevel)level);
        var result = await GetAllAsync(spec, ct);
        return result;
    }

    public async Task<IEnumerable<Course>> GetAvailableCoursesAsync(CancellationToken ct = default)
    {
        var spec = new AvailableCoursesSpec();
        var result = await GetAllAsync(spec, ct);
        return result;
    }

    public async Task<IEnumerable<Course>> GetByDepartmentAsync(Guid departmentId, CancellationToken ct = default)
    {
        var spec = new CourseByDepartmentSpec(departmentId);
        var result = await GetAllAsync(spec, ct);
        return result;
    }
}