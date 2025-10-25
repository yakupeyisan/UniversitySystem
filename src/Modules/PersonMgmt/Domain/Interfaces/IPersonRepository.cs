using Core.Domain.Repositories;
using PersonMgmt.Domain.Aggregates;
namespace PersonMgmt.Domain.Interfaces;
public interface IPersonRepository : IGenericRepository<Person>
{
    Task<bool> IsIdentificationNumberUniqueAsync(string identificationNumber, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> IsEmployeeNumberUniqueAsync(string employeeNumber, CancellationToken cancellationToken = default);
    Task<bool> IsStudentNumberUniqueAsync(string studentNumber, CancellationToken cancellationToken = default);

    Task<Person?> GetByIdentificationNumberAsync(string identificationNumber, CancellationToken cancellationToken = default);
    Task<Person?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Person?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<ICollection<Person>> SearchByNameAsync(string firstName, string lastName, CancellationToken cancellationToken = default);

    Task<Student?> GetStudentByStudentNumberAsync(string studentNumber, CancellationToken cancellationToken = default);
    Task<ICollection<Student>> GetStudentsByProgramAsync(Guid programId, CancellationToken cancellationToken = default);
    Task<ICollection<Student>> GetStudentsByGpaRangeAsync(double minGpa, double maxGpa, CancellationToken cancellationToken = default);

    Task<ICollection<Staff>> GetStaffByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default);
    Task<ICollection<Staff>> GetStaffByPositionAsync(string position, CancellationToken cancellationToken = default);

    Task<ICollection<PersonRestriction>> GetActiveRestrictionsAsync(Guid personId, CancellationToken cancellationToken = default);

    Task<ICollection<HealthRecord>> GetHealthRecordsByDateRangeAsync(Guid personId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}