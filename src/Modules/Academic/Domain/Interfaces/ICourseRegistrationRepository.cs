using Academic.Domain.Aggregates;
using Core.Domain.Repositories;
namespace Academic.Domain.Interfaces;
public interface ICourseRegistrationRepository : IRepository<CourseRegistration>
{
    Task<IEnumerable<CourseRegistration>> GetByStudentAsync(Guid studentId, CancellationToken ct = default);
    Task<CourseRegistration?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId, CancellationToken ct = default);
    Task<IEnumerable<CourseRegistration>> GetByCourseAsync(Guid courseId, CancellationToken ct = default);
    Task<IEnumerable<CourseRegistration>> GetByStudentAndSemesterAsync(Guid studentId, string semester, CancellationToken ct = default);
}