using Academic.Domain.Aggregates;
using Core.Domain.Repositories;
namespace Academic.Domain.Interfaces;
public interface IGradeObjectionRepository : IRepository<GradeObjection>
{
    Task<IEnumerable<GradeObjection>> GetByStudentAsync(Guid studentId, CancellationToken ct = default);
    Task<IEnumerable<GradeObjection>> GetByGradeAsync(Guid gradeId, CancellationToken ct = default);
    Task<IEnumerable<GradeObjection>> GetPendingObjectionsAsync(CancellationToken ct = default);
    Task<GradeObjection?> GetByStudentAndGradeAsync(Guid studentId, Guid gradeId, CancellationToken ct = default);
}