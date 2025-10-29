using Academic.Domain.Aggregates;
using Academic.Domain.Specifications;
using Shared.Infrastructure.Persistence.Contexts;

namespace Shared.Infrastructure.Persistence.Repositories.Academic;

public class WaitingListRepository : GenericRepository<CourseWaitingListEntry>
{
    public WaitingListRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<CourseWaitingListEntry>> GetByStudentAsync(Guid studentId,
        CancellationToken ct = default)
    {
        var spec = new WaitingListByStudentSpec(studentId);
        var result = await GetAllAsync(spec, ct);
        return result;
    }

    public async Task<IEnumerable<CourseWaitingListEntry>> GetByCourseOrderedByPositionAsync(Guid courseId,
        CancellationToken ct = default)
    {
        var spec = new WaitingListByCourseSpec(courseId);
        var result = await GetAllAsync(spec, ct);
        return result;
    }

    public async Task<CourseWaitingListEntry?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId,
        CancellationToken ct = default)
    {
        var spec = new WaitingListByStudentAndCourseSpec(studentId, courseId);
        return await GetAsync(spec, ct);
    }

    public async Task<int> GetNextQueuePositionAsync(Guid courseId, CancellationToken ct = default)
    {
        var spec = new WaitingListByCourseSpec(courseId);
        var result = await GetAllAsync(spec, ct);
        var entries = result.ToList();
        return entries.Any() ? entries.Max(e => e.QueuePosition) + 1 : 1;
    }

    public async Task<IEnumerable<CourseWaitingListEntry>> GetAdmittedEntriesAsync(Guid courseId,
        CancellationToken ct = default)
    {
        var spec = new WaitingListAdmittedEntriesByStatusSpec(courseId);
        var result = await GetAllAsync(spec, ct);
        return result;
    }
}