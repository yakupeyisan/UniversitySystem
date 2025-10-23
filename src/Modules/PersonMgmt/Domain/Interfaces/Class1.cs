using Core.Domain.Repositories;
using PersonMgmt.Domain.Aggregates;

namespace PersonMgmt.Domain.Interfaces;

/// <summary>
/// 🆕 NEW: IPersonRepository - Person Aggregate Root Repository
/// 
/// Sorumluluğu:
/// - Person aggregate'inin persistence'ını yönetmek
/// - Module-specific query methods sağlamak
/// - Unique constraints'leri check etmek
/// - Specification'ları support etmek
/// 
/// Kullanım:
/// var person = await _personRepository.GetByIdAsync(personId);
/// var byNationalId = await _personRepository.GetByNationalIdAsync("12345678901");
/// var isUnique = await _personRepository.IsNationalIdUniqueAsync("12345678901");
/// </summary>
public interface IPersonRepository : IRepository<Person>
{
    // ==================== BASIC QUERIES ====================

    /// <summary>
    /// T.C. Kimlik Numarası'na göre kişi getir
    /// </summary>
    Task<Person?> GetByNationalIdAsync(
        string nationalId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Silinmemiş kişileri getir
    /// </summary>
    Task<IEnumerable<Person>> GetAllActiveAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// E-posta adresine göre kişi getir
    /// </summary>
    Task<Person?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    // ==================== STUDENT QUERIES ====================

    /// <summary>
    /// Öğrenci numarası'na göre kişi (öğrenci) getir
    /// </summary>
    Task<Person?> GetByStudentNumberAsync(
        string studentNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirli danışmana atanmış öğrencileri getir
    /// </summary>
    Task<IEnumerable<Person>> GetStudentsByAdvisorAsync(
        Guid advisorId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirli durumda olan öğrencileri getir
    /// </summary>
    Task<IEnumerable<Person>> GetStudentsByStatusAsync(
        int status,  // StudentStatus enum value
        CancellationToken cancellationToken = default);

    // ==================== STAFF QUERIES ====================

    /// <summary>
    /// Personel numarası'na göre kişi (personel) getir
    /// </summary>
    Task<Person?> GetByEmployeeNumberAsync(
        string employeeNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirli akademik ünvana sahip personeli getir
    /// </summary>
    Task<IEnumerable<Person>> GetStaffByAcademicTitleAsync(
        int academicTitle,  // AcademicTitle enum value
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Aktif personeli getir
    /// </summary>
    Task<IEnumerable<Person>> GetActiveStaffAsync(
        CancellationToken cancellationToken = default);

    // ==================== UNIQUENESS CHECKS ====================

    /// <summary>
    /// National ID unique mi kontrol et
    /// excludeId: Bu ID dışında başka kişi tarafından kullanılıyor mu?
    /// </summary>
    Task<bool> IsNationalIdUniqueAsync(
        string nationalId,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Student Number unique mi kontrol et
    /// </summary>
    Task<bool> IsStudentNumberUniqueAsync(
        string studentNumber,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Employee Number unique mi kontrol et
    /// </summary>
    Task<bool> IsEmployeeNumberUniqueAsync(
        string employeeNumber,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Email unique mi kontrol et
    /// </summary>
    Task<bool> IsEmailUniqueAsync(
        string email,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    // ==================== RESTRICTION QUERIES ====================

    /// <summary>
    /// Belirli bir kısıtlama türüne sahip kişileri getir
    /// </summary>
    Task<IEnumerable<Person>> GetPersonsWithRestrictionTypeAsync(
        int restrictionType,  // RestrictionType enum value
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirli bir kısıtlama seviyesine sahip kişileri getir
    /// </summary>
    Task<IEnumerable<Person>> GetPersonsWithRestrictionLevelAsync(
        int restrictionLevel,  // RestrictionLevel enum value
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Aktif kısıtlaması olan kişileri getir
    /// </summary>
    Task<IEnumerable<Person>> GetPersonsWithActiveRestrictionsAsync(
        CancellationToken cancellationToken = default);

    // ==================== DEPARTMENT QUERIES ====================

    /// <summary>
    /// Belirli departmandaki kişileri getir
    /// </summary>
    Task<IEnumerable<Person>> GetPersonsByDepartmentAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirli departmandaki aktif öğrencileri getir
    /// </summary>
    Task<IEnumerable<Person>> GetActiveStudentsByDepartmentAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default);

    // ==================== HEALTH RECORD QUERIES ====================

    /// <summary>
    /// Sağlık kaydı olan kişileri getir
    /// </summary>
    Task<IEnumerable<Person>> GetPersonsWithHealthRecordAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirli kan grubuna sahip kişileri getir
    /// </summary>
    Task<IEnumerable<Person>> GetPersonsByBloodTypeAsync(
        string bloodType,
        CancellationToken cancellationToken = default);

    // ==================== SEARCH & FILTER ====================

    /// <summary>
    /// Ad veya soyada göre kişileri ara
    /// </summary>
    Task<IEnumerable<Person>> SearchByNameAsync(
        string searchTerm,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirli yaş aralığındaki kişileri getir
    /// </summary>
    Task<IEnumerable<Person>> GetPersonsByAgeRangeAsync(
        int minAge,
        int maxAge,
        CancellationToken cancellationToken = default);

    // ==================== ADVANCED QUERIES ====================

    /// <summary>
    /// Kayıt tarihine göre kişileri getir (paging destekler)
    /// </summary>
    Task<IEnumerable<Person>> GetPersonsRegisteredBetweenAsync(
        DateTime startDate,
        DateTime endDate,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Silinen kişileri dahil tüm kişileri getir (soft delete dahil)
    /// </summary>
    Task<IEnumerable<Person>> GetAllIncludingDeletedAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sadece silinen kişileri getir
    /// </summary>
    Task<IEnumerable<Person>> GetDeletedPersonsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sayfa sayfalı kişi listesi getir
    /// </summary>
    Task<(IEnumerable<Person> Items, int TotalCount)> GetPersonsPaginatedAsync(
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
}