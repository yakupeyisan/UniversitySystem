using Core.Domain.Repositories;
using PersonMgmt.Domain.Aggregates;
namespace PersonMgmt.Domain.Interfaces;
public interface IPersonRepository : IGenericRepository<Person>
{
    Task<Person?> GetByIdentificationNumberAsync(string identificationNumber);
    Task<Person?> GetByEmailAsync(string email);
    Task<Person?> GetByPhoneNumberAsync(string phoneNumber);
    Task<ICollection<Person>> SearchByNameAsync(string firstName, string lastName);
    Task<Student?> GetStudentByStudentNumberAsync(string studentNumber);
    Task<ICollection<Student>> GetStudentsByProgramAsync(Guid programId);
    Task<ICollection<Student>> GetStudentsByGpaRangeAsync(decimal minGpa, decimal maxGpa);
    Task<ICollection<Staff>> GetStaffByDepartmentAsync(Guid departmentId);
    Task<ICollection<Staff>> GetStaffByPositionAsync(string position);
    Task<ICollection<PersonRestriction>> GetActiveRestrictionsAsync(Guid personId);
    Task<ICollection<HealthRecord>> GetHealthRecordsByDateRangeAsync(Guid personId, DateTime startDate, DateTime endDate);
    Task<bool> IsNationalIdUniqueAsync(string requestNationalId, CancellationToken cancellationToken);
    Task<bool> IsEmailUniqueAsync(string requestEmail, CancellationToken cancellationToken, Guid? excludeId = null);
    Task<bool> IsEmployeeNumberUniqueAsync(string requestEmployeeNumber, CancellationToken cancellationToken);
    Task<bool> IsStudentNumberUniqueAsync(string requestStudentNumber, CancellationToken cancellationToken);
}