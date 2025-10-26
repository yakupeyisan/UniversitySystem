using Academic.Domain.Aggregates;
using Core.Domain.Repositories;

namespace Academic.Domain.Interfaces;

/// <summary>
/// Repository interface for Grade aggregate
/// </summary>
public interface IGradeRepository : IRepository<Grade>
{
    Task<IEnumerable<Grade>> GetByStudentAsync(Guid studentId, CancellationToken ct = default);
    Task<IEnumerable<Grade>> GetByStudentAndSemesterAsync(Guid studentId, string semester, CancellationToken ct = default);
    Task<Grade?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId, CancellationToken ct = default);
    Task<IEnumerable<Grade>> GetByRegistrationAsync(Guid registrationId, CancellationToken ct = default);
    Task<IEnumerable<Grade>> GetObjectedGradesAsync(Guid studentId, CancellationToken ct = default);
}