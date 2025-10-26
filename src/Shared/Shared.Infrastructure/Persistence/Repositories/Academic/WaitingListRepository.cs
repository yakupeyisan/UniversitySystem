using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using Academic.Domain.Interfaces;
using Academic.Domain.Specifications;
using Core.Domain.Specifications;
using Shared.Infrastructure.Persistence.Contexts;

namespace Shared.Infrastructure.Persistence.Repositories.Academic;

/// <summary>
/// Repository for CourseWaitingListEntry aggregate
/// </summary>
public class WaitingListRepository : GenericRepository<CourseWaitingListEntry>, IWaitingListRepository
{
    public WaitingListRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<CourseWaitingListEntry>> GetByStudentAsync(Guid studentId, CancellationToken ct = default)
    {
        var spec = new WaitingListByStudentSpec(studentId);
        var result = await GetAllAsync(spec, ct);
        return result.Data;
    }

    public async Task<IEnumerable<CourseWaitingListEntry>> GetByCourseOrderedByPositionAsync(Guid courseId, CancellationToken ct = default)
    {
        var spec = new WaitingListByCourseSpec(courseId);
        var result = await GetAllAsync(spec, ct);
        return result.Data;
    }

    public async Task<CourseWaitingListEntry?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId, CancellationToken ct = default)
    {
        var spec = new WaitingListByStudentAndCourseSpec(studentId, courseId);
        return await GetAsync(spec, ct);
    }

    public async Task<int> GetNextQueuePositionAsync(Guid courseId, CancellationToken ct = default)
    {
        var spec = new WaitingListByCourseSpec(courseId);
        var result = await GetAllAsync(spec, ct);
        var entries = result.Data.ToList();
        return entries.Any() ? entries.Max(e => e.QueuePosition) + 1 : 1;
    }

    public async Task<IEnumerable<CourseWaitingListEntry>> GetAdmittedEntriesAsync(Guid courseId, CancellationToken ct = default)
    {
        var spec = new WaitingListAdmittedEntriesByStatusSpec(courseId);
        var result = await GetAllAsync(spec, ct);
        return result.Data;
    }
}