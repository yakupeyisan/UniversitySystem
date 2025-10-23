using Core.Domain.Repositories;
using PersonMgmt.Domain.Aggregates;

namespace PersonMgmt.Domain.Interfaces;
/// <summary>
/// Person aggregate repository - Domain-specific operasyonlar
/// DDD: IGenericRepository'den türüyor + Person'a özel metodlar ekliyor
/// </summary>
public interface IPersonRepository : IGenericRepository<Person>
{
    // ==================== PERSON-SPECIFIC QUERIES ====================

    /// <summary>
    /// Kimlik numarasıyla kişi bulma (TR'de kimlik doğruluğu önemli)
    /// </summary>
    Task<Person?> GetByIdentificationNumberAsync(string identificationNumber);

    /// <summary>
    /// Email ile kişi bulma (unique constraint olmalı)
    /// </summary>
    Task<Person?> GetByEmailAsync(string email);

    /// <summary>
    /// Telefon numarasıyla kişi bulma
    /// </summary>
    Task<Person?> GetByPhoneNumberAsync(string phoneNumber);

    /// <summary>
    /// Adıyla kişi/kişileri arama (partial match)
    /// </summary>
    Task<ICollection<Person>> SearchByNameAsync(string firstName, string lastName);

    // ==================== STUDENT-SPECIFIC QUERIES ====================

    /// <summary>
    /// Öğrenci numarasıyla öğrenci bulma
    /// </summary>
    Task<Student?> GetStudentByStudentNumberAsync(string studentNumber);

    /// <summary>
    /// Programa kayıtlı öğrencileri getir
    /// </summary>
    Task<ICollection<Student>> GetStudentsByProgramAsync(Guid programId);

    /// <summary>
    /// GPA aralığında öğrencileri getir
    /// </summary>
    Task<ICollection<Student>> GetStudentsByGpaRangeAsync(decimal minGpa, decimal maxGpa);

    // ==================== STAFF-SPECIFIC QUERIES ====================

    /// <summary>
    /// Departmandaki personeli getir
    /// </summary>
    Task<ICollection<Staff>> GetStaffByDepartmentAsync(Guid departmentId);

    /// <summary>
    /// Görüşe göre personeli getir
    /// </summary>
    Task<ICollection<Staff>> GetStaffByPositionAsync(string position);

    // ==================== RESTRICTION-SPECIFIC QUERIES ====================

    /// <summary>
    /// Kişinin aktif kısıtlamalarını getir
    /// </summary>
    Task<ICollection<PersonRestriction>> GetActiveRestrictionsAsync(Guid personId);

    // ==================== HEALTH RECORD-SPECIFIC QUERIES ====================

    /// <summary>
    /// Kişinin sağlık kayıtlarını tarih aralığında getir
    /// </summary>
    Task<ICollection<HealthRecord>> GetHealthRecordsByDateRangeAsync(Guid personId, DateTime startDate, DateTime endDate);

    Task<bool> IsEmployeeNumberUniqueAsync(string requestEmployeeNumber, CancellationToken cancellationToken);
}