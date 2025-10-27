using Academic.Domain.Aggregates;
using Core.Domain.Repositories;
namespace Academic.Domain.Interfaces;
public interface IExamRepository : IRepository<Exam>
{
    Task<IEnumerable<Exam>> GetByCourseAsync(Guid courseId, CancellationToken ct = default);
    Task<IEnumerable<Exam>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate, CancellationToken ct = default);
    Task<IEnumerable<Exam>> GetByStudentAsync(Guid studentId, CancellationToken ct = default);
    Task<IEnumerable<Exam>> GetConflictingExamsAsync(Guid studentId, DateOnly examDate, TimeOnly startTime, TimeOnly endTime, CancellationToken ct = default);
}