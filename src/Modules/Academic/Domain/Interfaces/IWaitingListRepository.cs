using Academic.Domain.Aggregates;
using Core.Domain.Repositories;

namespace Academic.Domain.Interfaces;

/// <summary>
/// Repository interface for CourseWaitingListEntry aggregate
/// </summary>
public interface IWaitingListRepository : IRepository<CourseWaitingListEntry>
{
    Task<IEnumerable<CourseWaitingListEntry>> GetByStudentAsync(Guid studentId, CancellationToken ct = default);
    Task<IEnumerable<CourseWaitingListEntry>> GetByCourseOrderedByPositionAsync(Guid courseId, CancellationToken ct = default);
    Task<CourseWaitingListEntry?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId, CancellationToken ct = default);
    Task<int> GetNextQueuePositionAsync(Guid courseId, CancellationToken ct = default);
    Task<IEnumerable<CourseWaitingListEntry>> GetAdmittedEntriesAsync(Guid courseId, CancellationToken ct = default);
}