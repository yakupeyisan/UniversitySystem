using Academic.Domain.Aggregates;
using Academic.Domain.Interfaces;
using Academic.Domain.Specifications;
using Core.Domain.Specifications;
using Shared.Infrastructure.Persistence.Contexts;
namespace Shared.Infrastructure.Persistence.Repositories.Academic;
public class CourseRegistrationRepository : GenericRepository<CourseRegistration>, ICourseRegistrationRepository
{
    public CourseRegistrationRepository(AppDbContext context) : base(context) { }
    public async Task<IEnumerable<CourseRegistration>> GetByStudentAsync(Guid studentId, CancellationToken ct = default)
    {
        var spec = new CourseRegistrationByStudentSpec(studentId);
        var result = await GetAllAsync(spec, ct);
        return result.Data;
    }
    public async Task<CourseRegistration?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId, CancellationToken ct = default)
    {
        var spec = new CourseRegistrationByStudentAndCourseSpec(studentId, courseId);
        return await GetAsync(spec, ct);
    }
    public async Task<IEnumerable<CourseRegistration>> GetByCourseAsync(Guid courseId, CancellationToken ct = default)
    {
        var spec = new CourseRegistrationByCourseSpec(courseId);
        var result = await GetAllAsync(spec, ct);
        return result.Data;
    }
    public async Task<IEnumerable<CourseRegistration>> GetByStudentAndSemesterAsync(Guid studentId, string semester, CancellationToken ct = default)
    {
        var spec = new CourseRegistrationBySemesterSpec(studentId, semester);
        var result = await GetAllAsync(spec, ct);
        return result.Data;
    }
}