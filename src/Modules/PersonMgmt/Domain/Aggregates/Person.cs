using Core.Domain;
using PersonMgmt.Domain.Enums;
using PersonMgmt.Domain.Events;

namespace PersonMgmt.Domain.Aggregates;

/// <summary>
/// Person - Aggregate Root
/// 
/// Sorumluluğu:
/// - Kişi (Öğrenci, Öğretim Üyesi, Yönetici) bilgilerini yönet
/// - Child entities'leri (Student, Staff, HealthRecord, Restrictions) kontrol et
/// - Consistency boundary'sini sağla
/// - Domain events'i raise et
/// 
/// Yapı:
/// - Student ve Staff arasında ONE-TO-ONE (bir kişi hem öğrenci hem personel olamaz)
/// - HealthRecord: ONE-TO-ONE
/// - Restrictions: ONE-TO-MANY
/// 
/// NotExceptions:
/// - PersonNotFoundException
/// - DuplicateNationalIdException
/// - DuplicateStudentNumberException
/// - DuplicateEmployeeNumberException
/// - PersonIsNotStudentException
/// - PersonIsNotStaffException
/// </summary>
public class Person : AggregateRoot
{
    /// <summary>
    /// Departman ID (opsiyonel)
    /// </summary>
    public Guid? DepartmentId { get; private set; }

    /// <summary>
    /// Ad ve soyad (ValueObject)
    /// </summary>
    public PersonName Name { get; private set; }

    /// <summary>
    /// T.C. Kimlik Numarası (unique)
    /// </summary>
    public string NationalId { get; private set; }

    /// <summary>
    /// Doğum tarihi
    /// </summary>
    public DateTime BirthDate { get; private set; }

    /// <summary>
    /// Cinsiyet
    /// </summary>
    public Gender Gender { get; private set; }

    /// <summary>
    /// E-posta
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    /// Telefon numarası
    /// </summary>
    public string PhoneNumber { get; private set; }

    /// <summary>
    /// Profil fotoğrafı URL'si
    /// </summary>
    public string? ProfilePhotoUrl { get; private set; }

    // ==================== CHILD ENTITIES ====================

    /// <summary>
    /// Öğrenci bilgileri (null = öğrenci değil)
    /// </summary>
    private Student? _student;
    public Student? Student => _student;

    /// <summary>
    /// Personel bilgileri (null = personel değil)
    /// </summary>
    private Staff? _staff;
    public Staff? Staff => _staff;

    /// <summary>
    /// Sağlık kaydı
    /// </summary>
    private HealthRecord? _healthRecord;
    public HealthRecord? HealthRecord => _healthRecord;

    /// <summary>
    /// Kişi üzerine konulan kısıtlamalar
    /// </summary>
    private readonly List<PersonRestriction> _restrictions = new();
    public IReadOnlyCollection<PersonRestriction> Restrictions => _restrictions.AsReadOnly();

    // ==================== AUDIT FIELDS ====================

    /// <summary>
    /// Soft delete
    /// </summary>
    public bool IsDeleted { get; private set; }

    /// <summary>
    /// Oluşturulma tarihi
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Güncelleme tarihi
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Silinme tarihi
    /// </summary>
    public DateTime? DeletedAt { get; private set; }

    /// <summary>
    /// Private constructor
    /// </summary>
    private Person()
    {
    }

    // ==================== FACTORY METHODS ====================

    /// <summary>
    /// Factory method - Yeni kişi oluştur
    /// </summary>
    public static Person Create(
        string firstName,
        string lastName,
        string nationalId,
        DateTime birthDate,
        Gender gender,
        string email,
        string phoneNumber,
        Guid? departmentId = null,
        string? profilePhotoUrl = null)
    {
        ValidateBasicInfo(firstName, lastName, nationalId, email, phoneNumber);
        ValidateBirthDate(birthDate);

        var person = new Person
        {
            Id = Guid.NewGuid(),
            DepartmentId = departmentId,
            Name = PersonName.Create(firstName, lastName),
            NationalId = nationalId,
            BirthDate = birthDate,
            Gender = gender,
            Email = email,
            PhoneNumber = phoneNumber,
            ProfilePhotoUrl = profilePhotoUrl,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Raise event
        person.AddDomainEvent(new PersonCreatedDomainEvent(
            person.Id,
            firstName,
            lastName,
            email,
            person.CreatedAt
        ));

        return person;
    }

    // ==================== BUSINESS LOGIC ====================

    /// <summary>
    /// Kişiyi öğrenci olarak kayıt et
    /// </summary>
    public void EnrollAsStudent(
        string studentNumber,
        EducationLevel educationLevel,
        DateTime enrollmentDate,
        Guid? advisorId = null)
    {
        if (_staff != null)
            throw new InvalidOperationException("Person is already registered as staff");

        if (_student != null)
            throw new InvalidOperationException("Person is already enrolled as student");

        _student = Student.Create(studentNumber, educationLevel, enrollmentDate, advisorId);
        UpdatedAt = DateTime.UtcNow;

        // Raise event
        AddDomainEvent(new StudentEnrolledDomainEvent(
            Id,
            studentNumber,
            (int)educationLevel,
            enrollmentDate
        ));
    }

    /// <summary>
    /// Kişiyi personel olarak kayıt et
    /// </summary>
    public void HireAsStaff(
        string employeeNumber,
        AcademicTitle academicTitle,
        DateTime hireDate,
        Address? address = null,
        EmergencyContact? emergencyContact = null)
    {
        if (_student != null)
            throw new InvalidOperationException("Person is already enrolled as student");

        if (_staff != null)
            throw new InvalidOperationException("Person is already registered as staff");

        _staff = Staff.Create(employeeNumber, academicTitle, hireDate, address, emergencyContact);
        UpdatedAt = DateTime.UtcNow;

        // Raise event
        AddDomainEvent(new StaffHiredDomainEvent(
            Id,
            employeeNumber,
            (int)academicTitle,
            hireDate
        ));
    }

    /// <summary>
    /// Sağlık kaydını oluştur/güncelle
    /// </summary>
    public void CreateOrUpdateHealthRecord(
        string? bloodType = null,
        string? allergies = null,
        string? chronicDiseases = null,
        string? medications = null,
        string? emergencyHealthInfo = null,
        string? notes = null)
    {
        if (_healthRecord == null)
        {
            _healthRecord = HealthRecord.Create(
                bloodType, allergies, chronicDiseases, medications, emergencyHealthInfo, notes
            );
        }
        else
        {
            if (!string.IsNullOrEmpty(bloodType))
                _healthRecord.UpdateBloodType(bloodType);
            if (!string.IsNullOrEmpty(allergies))
                _healthRecord.UpdateAllergies(allergies);
            if (!string.IsNullOrEmpty(chronicDiseases))
                _healthRecord.UpdateChronicDiseases(chronicDiseases);
            if (!string.IsNullOrEmpty(medications))
                _healthRecord.UpdateMedications(medications);
            if (!string.IsNullOrEmpty(emergencyHealthInfo))
                _healthRecord.UpdateEmergencyHealthInfo(emergencyHealthInfo);
            if (!string.IsNullOrEmpty(notes))
                _healthRecord.UpdateNotes(notes);
        }

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Kişi bilgilerini güncelle
    /// </summary>
    public void UpdatePersonalInfo(
        string email,
        string phoneNumber,
        Guid? departmentId = null,
        string? profilePhotoUrl = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

        Email = email;
        PhoneNumber = phoneNumber;
        DepartmentId = departmentId;
        ProfilePhotoUrl = profilePhotoUrl;
        UpdatedAt = DateTime.UtcNow;

        // Raise event
        AddDomainEvent(new PersonUpdatedDomainEvent(
            Id,
            Name.FirstName,
            Name.LastName,
            Email,
            UpdatedAt
        ));
    }

    /// <summary>
    /// Kısıtlama ekle
    /// </summary>
    public void AddRestriction(
        RestrictionType restrictionType,
        RestrictionLevel restrictionLevel,
        Guid appliedBy,
        DateTime startDate,
        DateTime? endDate,
        string reason,
        int severity)
    {
        var restriction = PersonRestriction.Create(
            restrictionType,
            restrictionLevel,
            appliedBy,
            startDate,
            endDate,
            reason,
            severity
        );

        _restrictions.Add(restriction);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Kısıtlamayı kaldır
    /// </summary>
    public void RemoveRestriction(Guid restrictionId)
    {
        var restriction = _restrictions.FirstOrDefault(r => r.Id == restrictionId);
        if (restriction == null)
            throw new InvalidOperationException("Restriction not found");

        restriction.Delete();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Etkin kısıtlamaları getir
    /// </summary>
    public IEnumerable<PersonRestriction> GetActiveRestrictions()
    {
        return _restrictions.Where(r => r.IsCurrentlyActive()).ToList();
    }

    /// <summary>
    /// Belirli seviyede aktif kısıtlama var mı?
    /// </summary>
    public bool HasActiveRestrictionAtLevel(RestrictionLevel level)
    {
        return _restrictions.Any(r =>
            r.IsCurrentlyActive() && r.RestrictionLevel == level
        );
    }

    /// <summary>
    /// Profil fotoğrafını güncelle
    /// </summary>
    public void UpdateProfilePhoto(string photoUrl)
    {
        if (string.IsNullOrWhiteSpace(photoUrl))
            throw new ArgumentException("Photo URL cannot be empty", nameof(photoUrl));

        ProfilePhotoUrl = photoUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft delete - Kişiyi sil
    /// </summary>
    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        // Delete child entities
        _student?.Delete();
        _staff?.Delete();
        _healthRecord?.Delete();

        // Clear restrictions
        foreach (var restriction in _restrictions)
        {
            restriction.Delete();
        }

        // Raise event
        AddDomainEvent(new PersonDeletedDomainEvent(
            Id,
            Name.FirstName,
            Name.LastName,
            DateTime.UtcNow
        ));
    }

    /// <summary>
    /// Soft delete geri al
    /// </summary>
    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        UpdatedAt = DateTime.UtcNow;

        // Restore child entities
        _student?.Restore();
        _staff?.Restore();
        _healthRecord?.Restore();
    }

    // ==================== VALIDATION HELPERS ====================

    private static void ValidateBasicInfo(
        string firstName,
        string lastName,
        string nationalId,
        string email,
        string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));
        if (string.IsNullOrWhiteSpace(nationalId))
            throw new ArgumentException("National ID cannot be empty", nameof(nationalId));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

        // National ID: 11 haneli, sadece rakam
        if (nationalId.Length != 11 || !nationalId.All(char.IsDigit))
            throw new ArgumentException("National ID must be 11 digits", nameof(nationalId));

        // Email validation
        if (!email.Contains("@") || !email.Contains("."))
            throw new ArgumentException("Invalid email format", nameof(email));

        // Phone: minimum 10 haneli
        if (phoneNumber.Replace("-", "").Replace(" ", "").Length < 10)
            throw new ArgumentException("Phone number must be at least 10 digits", nameof(phoneNumber));
    }

    private static void ValidateBirthDate(DateTime birthDate)
    {
        if (birthDate > DateTime.UtcNow)
            throw new ArgumentException("Birth date cannot be in the future", nameof(birthDate));

        var age = DateTime.UtcNow.Year - birthDate.Year;
        if (age < 15 || age > 100)
            throw new ArgumentException("Age must be between 15 and 100", nameof(birthDate));
    }
}