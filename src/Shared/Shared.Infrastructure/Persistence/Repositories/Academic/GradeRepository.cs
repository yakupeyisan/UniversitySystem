using Academic.Domain.Aggregates;
using Academic.Domain.Interfaces;
using Academic.Domain.Specifications;
using Shared.Infrastructure.Persistence.Contexts;
namespace Shared.Infrastructure.Persistence.Repositories.Academic;
public class GradeRepository : GenericRepository<Grade>, IGradeRepository
{
    public GradeRepository(AppDbContext context) : base(context) { }
    public async Task<IEnumerable<Grade>> GetByStudentAsync(Guid studentId, CancellationToken ct = default)
    {
        var spec = new GradesByStudentSpec(studentId);
        var result = await GetAllAsync(spec, ct);
        return result.Data;
    }
    public async Task<IEnumerable<Grade>> GetByStudentAndSemesterAsync(Guid studentId, string semester, CancellationToken ct = default)
    {
        var spec = new GradesByStudentBySemesterSpec(studentId, semester);
        var result = await GetAllAsync(spec, ct);
        return result.Data;
    }
    public async Task<Grade?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId, CancellationToken ct = default)
    {
        var spec = new GradeByStudentAndCourseSpec(studentId, courseId);
        return await GetAsync(spec, ct);
    }
    public async Task<IEnumerable<Grade>> GetByRegistrationAsync(Guid registrationId, CancellationToken ct = default)
    {
        var spec = new GradeByRegistrationSpec(registrationId);
        var result = await GetAllAsync(spec, ct);
        return result.Data;
    }
    public async Task<IEnumerable<Grade>> GetObjectedGradesAsync(Guid studentId, CancellationToken ct = default)
    {
        var spec = new ObjectedGradesByStudentSpec(studentId);
        var result = await GetAllAsync(spec, ct);
        return result.Data;
    }
}