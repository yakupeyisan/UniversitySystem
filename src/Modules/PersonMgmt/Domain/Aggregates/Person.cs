using Core.Domain;
using PersonMgmt.Domain.Enums;
using PersonMgmt.Domain.Events;
using PersonMgmt.Domain.ValueObjects;

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
        // Gelecek tarih olamaz
        if (birthDate > DateTime.UtcNow)
            throw new ArgumentException("Birth date cannot be in the future", nameof(birthDate));

        // Çok eski tarih olamaz (150 yıldan daha eski)
        var maxBirthDate = DateTime.UtcNow.AddYears(-150);
        if (birthDate < maxBirthDate)
            throw new ArgumentException("Birth date is unrealistic (too old)", nameof(birthDate));

        // En az 18 yaşında olmalı (opsiyonel - business rule'a göre değişebilir)
        var minBirthDate = DateTime.UtcNow.AddYears(-18);
        if (birthDate > minBirthDate)
            throw new ArgumentException("Person must be at least 18 years old", nameof(birthDate));
    }
    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        // Basic email format validation
        if (!email.Contains("@") || !email.Contains("."))
            throw new ArgumentException("Email format is invalid", nameof(email));

        // @ işaretinden sonra en az bir nokta olmalı
        var atIndex = email.IndexOf("@");
        var lastDotIndex = email.LastIndexOf(".");

        if (atIndex > lastDotIndex)
            throw new ArgumentException("Email format is invalid", nameof(email));

        // @ işaretinden en az 1 karakter uzaklıkta nokta
        if (lastDotIndex - atIndex < 2)
            throw new ArgumentException("Email domain is invalid", nameof(email));

        // TLD (Top Level Domain) en az 2 karakter
        if (email.Length - lastDotIndex - 1 < 2)
            throw new ArgumentException("Email domain extension is invalid", nameof(email));

        // Email length
        if (email.Length > 254)
            throw new ArgumentException("Email is too long (max 254 characters)", nameof(email));

        // Spaces not allowed
        if (email.Contains(" "))
            throw new ArgumentException("Email cannot contain spaces", nameof(email));
    }
    private static void ValidatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

        // Sadece rakam ve +, -, ( ), boşluk allowed
        var allowedChars = "0123456789+- ()";
        if (phoneNumber.Any(c => !allowedChars.Contains(c)))
            throw new ArgumentException("Phone number contains invalid characters", nameof(phoneNumber));

        // En az 10 rakam olmalı (ülke kodları dahil)
        var digitCount = phoneNumber.Count(char.IsDigit);
        if (digitCount < 10)
            throw new ArgumentException("Phone number must have at least 10 digits", nameof(phoneNumber));

        // En fazla 15 rakam (E.164 standardı)
        if (digitCount > 15)
            throw new ArgumentException("Phone number has too many digits (max 15)", nameof(phoneNumber));

        // Türkiye numarası ise kontrol
        if (phoneNumber.StartsWith("+90") || phoneNumber.StartsWith("0090") || phoneNumber.StartsWith("90"))
        {
            if (digitCount != 12)  // +90 + 10 rakam = 12
                throw new ArgumentException("Turkish phone number must have 10 digits after +90", nameof(phoneNumber));
        }
    }
    private static void ValidateNationalId(string nationalId)
    {
        if (string.IsNullOrWhiteSpace(nationalId))
            throw new ArgumentException("National ID cannot be empty", nameof(nationalId));

        // Türk T.C. Kimlik Numarası: 11 hane, sadece rakam
        if (nationalId.Length != 11 || !nationalId.All(char.IsDigit))
            throw new ArgumentException("National ID must be 11 digits", nameof(nationalId));

        // İlk rakam 0 olamaz
        if (nationalId[0] == '0')
            throw new ArgumentException("National ID cannot start with 0", nameof(nationalId));

        // Luhn algorithm validation (Türk ID checksum'ı)
        // Not: Türk TC Kimlik No'sunun gerçek validation'ı daha karmaşık
        // Basit check: 10. ve 11. rakamlar belli kurallar izler
        // Burada simplified version:
        int sum = 0;
        for (int i = 0; i < 10; i++)
        {
            sum += int.Parse(nationalId[i].ToString());
        }

        // 10. rakam (index 9): ilk 10 rakamın toplamının son rakamı
        int tenthDigitCheck = sum % 10;
        if (int.Parse(nationalId[9].ToString()) != tenthDigitCheck)
            throw new ArgumentException("National ID checksum validation failed", nameof(nationalId));
    }
    public static void ValidateStudentNumber(string studentNumber)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
            throw new ArgumentException("Student number cannot be empty", nameof(studentNumber));

        if (studentNumber.Length < 8 || studentNumber.Length > 12)
            throw new ArgumentException("Student number must be between 8-12 characters", nameof(studentNumber));

        // Alfanumerik allowed
        if (!studentNumber.All(c => char.IsLetterOrDigit(c)))
            throw new ArgumentException("Student number must be alphanumeric", nameof(studentNumber));
    }
    public static void ValidateEmployeeNumber(string employeeNumber)
    {
        if (string.IsNullOrWhiteSpace(employeeNumber))
            throw new ArgumentException("Employee number cannot be empty", nameof(employeeNumber));

        if (employeeNumber.Length < 5 || employeeNumber.Length > 10)
            throw new ArgumentException("Employee number must be between 5-10 characters", nameof(employeeNumber));

        // Alfanumerik allowed
        if (!employeeNumber.All(c => char.IsLetterOrDigit(c)))
            throw new ArgumentException("Employee number must be alphanumeric", nameof(employeeNumber));
    }

}