using System.Text.RegularExpressions;
using Core.Domain;
using Core.Domain.Specifications;
using PersonMgmt.Domain.Enums;
using PersonMgmt.Domain.Events;
using PersonMgmt.Domain.ValueObjects;
namespace PersonMgmt.Domain.Aggregates;
public class Person : AuditableEntity, ISoftDelete
{
    private readonly List<Address> _addresses = new();
    private readonly List<EmergencyContact> _emergencyContacts = new();
    private readonly List<PersonRestriction> _restrictions = new();
    private Person()
    {
    }
    public Guid? DepartmentId { get; private set; }
    public PersonName Name { get; private set; } = null!;
    public string IdentificationNumber { get; private set; } = null!;
    public DateTime BirthDate { get; private set; }
    public Gender Gender { get; private set; }
    public string Email { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public string? ProfilePhotoUrl { get; private set; }
    public Student? Student { get; private set; }
    public Staff? Staff { get; private set; }
    public HealthRecord? HealthRecord { get; private set; }
    public IReadOnlyList<Address> Addresses => _addresses.AsReadOnly();
    public IReadOnlyList<EmergencyContact> EmergencyContacts => _emergencyContacts.AsReadOnly();
    public IReadOnlyCollection<PersonRestriction> Restrictions => _restrictions.AsReadOnly();
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }
    public void Delete(Guid deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        Student?.Delete(deletedBy);
        Staff?.Delete(deletedBy);
        HealthRecord?.Delete(deletedBy);
        foreach (var address in _addresses) address.Delete(deletedBy);
        foreach (var restriction in _restrictions) restriction.Delete(deletedBy);
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
        Student?.Restore();
        Staff?.Restore();
        HealthRecord?.Restore();
        foreach (var address in _addresses) address.Restore();
        foreach (var restriction in _restrictions) restriction.Restore();
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
    public Address? GetCurrentAddress()
    {
        return _addresses.FirstOrDefault(a => a.IsCurrent && a.IsActive);
    }
    public IEnumerable<Address> GetAddressHistory()
    {
        return _addresses.Where(a => !a.IsCurrent && !a.IsDeleted).OrderByDescending(a => a.ValidTo);
    }
    public void AddAddress(Address address)
    {
        if (address.PersonId != Id)
            throw new InvalidOperationException("Address does not belong to this person");
        foreach (var oldAddress in _addresses.Where(a => a.IsCurrent && !a.IsDeleted)) oldAddress.Archive();
        _addresses.Add(address);
        UpdatedAt = DateTime.UtcNow;
    }
    public void AddTurkishAddress(string street, string city, string? postalCode = null)
    {
        var address = Address.CreateTurkish(Id, street, city, postalCode);
        AddAddress(address);
    }
    public void DeleteAddress(Guid addressId, Guid deletedBy)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId);
        if (address == null)
            throw new InvalidOperationException("Address not found");
        address.Delete(deletedBy);
        UpdatedAt = DateTime.UtcNow;
    }
    public void RestoreAddress(Guid addressId)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId);
        if (address == null)
            throw new InvalidOperationException("Address not found");
        address.Restore();
        UpdatedAt = DateTime.UtcNow;
    }
    public void EnrollAsStudent(
        string studentNumber,
        EducationLevel educationLevel,
        DateTime enrollmentDate,
        Guid? advisorId = null,
        Guid? programId = null)
    {
        if (Staff != null)
            throw new InvalidOperationException("Person is already registered as staff");
        if (Student != null)
            throw new InvalidOperationException("Person is already enrolled as student");
        Student = Student.Create(studentNumber, educationLevel, enrollmentDate, advisorId, programId);
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
        if (Student != null)
            throw new InvalidOperationException("Person is already enrolled as student");
        if (Staff != null)
            throw new InvalidOperationException("Person is already registered as staff");
        Staff = Staff.Create(employeeNumber, academicTitle, hireDate);
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
        if (HealthRecord == null)
        {
            HealthRecord = HealthRecord.Create(
                Id, bloodType, allergies, chronicDiseases, medications, emergencyHealthInfo, notes
            );
        }
        else
        {
            if (!string.IsNullOrEmpty(bloodType))
                HealthRecord.UpdateBloodType(bloodType);
            if (!string.IsNullOrEmpty(allergies))
                HealthRecord.UpdateAllergies(allergies);
            if (!string.IsNullOrEmpty(chronicDiseases))
                HealthRecord.UpdateChronicDiseases(chronicDiseases);
            if (!string.IsNullOrEmpty(medications))
                HealthRecord.UpdateMedications(medications);
            if (!string.IsNullOrEmpty(emergencyHealthInfo))
                HealthRecord.UpdateEmergencyHealthInfo(emergencyHealthInfo);
            if (!string.IsNullOrEmpty(notes))
                HealthRecord.UpdateNotes(notes);
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
            UpdatedAt.Value
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