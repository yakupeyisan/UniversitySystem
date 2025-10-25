using Core.Domain;
using Core.Domain.Specifications;
using PersonMgmt.Domain.Enums;
using PersonMgmt.Domain.Events;
using PersonMgmt.Domain.ValueObjects;
using System.Text.RegularExpressions;
namespace PersonMgmt.Domain.Aggregates;

public class Person : AggregateRoot, ISoftDelete
{
    public Guid? DepartmentId { get; private set; }
    public PersonName Name { get; private set; } = null!;
    public string IdentificationNumber { get; private set; } = null!;
    public DateTime BirthDate { get; private set; }
    public Gender Gender { get; private set; }
    public string Email { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public string? ProfilePhotoUrl { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }

    // Navigation properties
    private Student? _student;
    public Student? Student => _student;

    private Staff? _staff;
    public Staff? Staff => _staff;

    private HealthRecord? _healthRecord;
    public HealthRecord? HealthRecord => _healthRecord;

    // ✅ Address koleksiyonu - birden çok adres ve geçmiş
    private readonly List<Address> _addresses = new();
    public IReadOnlyList<Address> Addresses => _addresses.AsReadOnly();
    private readonly List<EmergencyContact> _emergencyContacts = new();
    public IReadOnlyList<EmergencyContact> EmergencyContacts => _emergencyContacts.AsReadOnly();

    private readonly List<PersonRestriction> _restrictions = new();
    public IReadOnlyCollection<PersonRestriction> Restrictions => _restrictions.AsReadOnly();

    private Person()
    {
    }

    public static Person Create(
        string firstName,
        string lastName,
        string identificationNumber,
        DateTime birthDate,
        Gender gender,
        string email,
        string phoneNumber,
        Guid? departmentId = null,
        string? profilePhotoUrl = null)
    {
        ValidateBasicInfo(firstName, lastName, identificationNumber, email, phoneNumber);
        ValidateBirthDate(birthDate);

        var person = new Person
        {
            Id = Guid.NewGuid(),
            DepartmentId = departmentId,
            Name = PersonName.Create(firstName, lastName),
            IdentificationNumber = identificationNumber,
            BirthDate = birthDate,
            Gender = gender,
            Email = email,
            PhoneNumber = phoneNumber,
            ProfilePhotoUrl = profilePhotoUrl,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        person.AddDomainEvent(new PersonCreatedDomainEvent(
            person.Id,
            firstName,
            lastName,
            email,
            person.CreatedAt
        ));

        return person;
    }

    /// <summary>
    /// Kişinin şu anki (aktif) adresini al
    /// </summary>
    public Address? GetCurrentAddress()
    {
        return _addresses.FirstOrDefault(a => a.IsCurrent && a.IsActive);
    }

    /// <summary>
    /// Kişinin tüm adres geçmişini al (archive edilmiş adresler)
    /// </summary>
    public IEnumerable<Address> GetAddressHistory()
    {
        return _addresses.Where(a => !a.IsCurrent && !a.IsDeleted).OrderByDescending(a => a.ValidTo);
    }

    /// <summary>
    /// Yeni bir adres ekle (eski adresler otomatik archive edilir)
    /// </summary>
    public void AddAddress(Address address)
    {
        if (address.PersonId != Id)
            throw new InvalidOperationException("Address does not belong to this person");

        // Eski aktif adresler archive et
        foreach (var oldAddress in _addresses.Where(a => a.IsCurrent && !a.IsDeleted))
        {
            oldAddress.Archive();
        }

        _addresses.Add(address);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adres ekle (Türkiye için convenience method)
    /// </summary>
    public void AddTurkishAddress(string street, string city, string? postalCode = null)
    {
        var address = Address.CreateTurkish(Id, street, city, postalCode);
        AddAddress(address);
    }

    /// <summary>
    /// Belirli bir adresi sil (soft delete)
    /// </summary>
    public void DeleteAddress(Guid addressId)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId);
        if (address == null)
            throw new InvalidOperationException("Address not found");

        address.Delete();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Belirli bir adresi geri yükle
    /// </summary>
    public void RestoreAddress(Guid addressId)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId);
        if (address == null)
            throw new InvalidOperationException("Address not found");

        address.Restore();
        UpdatedAt = DateTime.UtcNow;
    }

    // Student, Staff, HealthRecord, Restrictions ve diğer methodlar aynı kalır
    // (Kısitlama: Tam dosya çok uzun olacağından, bu dosyada sadece Address ile ilgili kısımları gösterdim)
    // Orijinal Person.cs'daki tüm methodlar korunmalıdır

    public void EnrollAsStudent(
        string studentNumber,
        EducationLevel educationLevel,
        DateTime enrollmentDate,
        Guid? advisorId = null,
        Guid? programId = null)
    {
        if (_staff != null)
            throw new InvalidOperationException("Person is already registered as staff");
        if (_student != null)
            throw new InvalidOperationException("Person is already enrolled as student");

        _student = Student.Create(studentNumber, educationLevel, enrollmentDate, advisorId, programId);
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new StudentEnrolledDomainEvent(
            Id,
            studentNumber,
            (int)educationLevel,
            enrollmentDate
        ));
    }

    public void HireAsStaff(
        string employeeNumber,
        AcademicTitle academicTitle,
        DateTime hireDate)
    {
        if (_student != null)
            throw new InvalidOperationException("Person is already enrolled as student");
        if (_staff != null)
            throw new InvalidOperationException("Person is already registered as staff");

        _staff = Staff.Create(employeeNumber, academicTitle, hireDate);
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new StaffHiredDomainEvent(
            Id,
            employeeNumber,
            (int)academicTitle,
            hireDate
        ));
    }

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
                Id, bloodType, allergies, chronicDiseases, medications, emergencyHealthInfo, notes
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
        AddDomainEvent(new PersonUpdatedDomainEvent(
            Id,
            Name.FirstName,
            Name.LastName,
            Email,
            UpdatedAt
        ));
    }

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
            Id,
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

    public void RemoveRestriction(Guid restrictionId, Guid deletedBy)
    {
        var restriction = _restrictions.FirstOrDefault(r => r.Id == restrictionId);
        if (restriction == null)
            throw new InvalidOperationException("Restriction not found");

        restriction.Delete(deletedBy);
        UpdatedAt = DateTime.UtcNow;
    }

    public IEnumerable<PersonRestriction> GetActiveRestrictions()
    {
        return _restrictions.Where(r => r.IsCurrentlyActive()).ToList();
    }

    public bool HasActiveRestrictionAtLevel(RestrictionLevel level)
    {
        return _restrictions.Any(r =>
            r.IsCurrentlyActive() && r.RestrictionLevel == level
        );
    }

    public void UpdateProfilePhoto(string photoUrl)
    {
        if (string.IsNullOrWhiteSpace(photoUrl))
            throw new ArgumentException("Photo URL cannot be empty", nameof(photoUrl));

        ProfilePhotoUrl = photoUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete(Guid deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;

        _student?.Delete(deletedBy);
        _staff?.Delete(deletedBy);
        _healthRecord?.Delete();

        foreach (var address in _addresses)
        {
            address.Delete();
        }

        foreach (var restriction in _restrictions)
        {
            restriction.Delete(deletedBy);
        }

        AddDomainEvent(new PersonDeletedDomainEvent(
            Id,
            Name.FirstName,
            Name.LastName,
            DateTime.UtcNow
        ));
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        UpdatedAt = DateTime.UtcNow;

        _student?.Restore();
        _staff?.Restore();
        _healthRecord?.Restore();

        foreach (var address in _addresses)
        {
            address.Restore();
        }
    }

    #region Validation

    private static void ValidateBasicInfo(
        string firstName,
        string lastName,
        string identificationNumber,
        string email,
        string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));
        if (string.IsNullOrWhiteSpace(identificationNumber))
            throw new ArgumentException("Identification number cannot be empty", nameof(identificationNumber));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

        ValidateEmail(email);
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        if (!email.Contains("@") || !email.Contains("."))
            throw new ArgumentException("Email format is invalid", nameof(email));

        var emailRegex = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
        if (!Regex.IsMatch(email, emailRegex))
            throw new ArgumentException("Email format is invalid", nameof(email));

        if (email.Length > 100)
            throw new ArgumentException("Email cannot exceed 100 characters", nameof(email));

        if (email.StartsWith(".") || email.EndsWith("."))
            throw new ArgumentException("Email format is invalid", nameof(email));
    }

    private static void ValidateBirthDate(DateTime birthDate)
    {
        if (birthDate > DateTime.UtcNow)
            throw new ArgumentException("Birth date cannot be in the future", nameof(birthDate));
    }

    #endregion
}


