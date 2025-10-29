using Academic.Domain.Aggregates;
using Academic.Domain.Specifications;
using Academic.Domain.ValueObjects;
using Shared.Infrastructure.Persistence.Contexts;

namespace Shared.Infrastructure.Persistence.Repositories.Academic;

public class ExamRepository : GenericRepository<Exam>, IExamRepository
{
    public ExamRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Exam>> GetByCourseAsync(Guid courseId, CancellationToken ct = default)
    {
        var spec = new ExamByCourseSpec(courseId);
        var result = await GetAllAsync(spec, ct);
        return result;
    }

    public async Task<IEnumerable<Exam>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate,
        CancellationToken ct = default)
    {
        var spec = new ExamsByDateRangeSpec(startDate, endDate);
        var result = await GetAllAsync(spec, ct);
        return result;
    }

    public async Task<IEnumerable<Exam>> GetByStudentAsync(Guid studentId, CancellationToken ct = default)
    {
        var spec = new ExamsByStudentSpec(studentId);
        var result = await GetAllAsync(spec, ct);
        return result;
    }

    public async Task<IEnumerable<Exam>> GetConflictingExamsAsync(
        Guid studentId,
        DateOnly examDate,
        TimeOnly startTime,
        TimeOnly endTime,
        CancellationToken ct = default)
    {
        var spec = new NotCancelledExamsSpec();
        var result = await GetAllAsync(spec, ct);
        var conflicting = result
            .Where(e => e.ExamDate == examDate &&
                        e.TimeSlot.OverlapsWith(TimeSlot.Create(startTime, endTime)))
            .ToList();
        return conflicting;
    }
}