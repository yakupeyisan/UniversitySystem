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
    public PersonName Name { get; private set; }
    public string NationalId { get; private set; }
    public DateTime BirthDate { get; private set; }
    public Gender Gender { get; private set; }
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public string? ProfilePhotoUrl { get; private set; }
    private Student? _student;
    public Student? Student => _student;
    private Staff? _staff;
    public Staff? Staff => _staff;
    private HealthRecord? _healthRecord;
    public HealthRecord? HealthRecord => _healthRecord;
    public Address Address { get; private set; }
    private readonly List<PersonRestriction> _restrictions = new();
    public IReadOnlyCollection<PersonRestriction> Restrictions => _restrictions.AsReadOnly();
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }
    private Person()
    {
    }
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
        person.AddDomainEvent(new PersonCreatedDomainEvent(
            person.Id,
            firstName,
            lastName,
            email,
            person.CreatedAt
        ));
        return person;
    }
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
    }
    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        if (!email.Contains("@") || !email.Contains("."))
            throw new ArgumentException("Email format is invalid", nameof(email));
        var atIndex = email.IndexOf("@");
        var lastDotIndex = email.LastIndexOf(".");
        if (atIndex > lastDotIndex)
            throw new ArgumentException("Email format is invalid", nameof(email));
        if (lastDotIndex - atIndex < 2)
            throw new ArgumentException("Email domain is invalid", nameof(email));
        if (email.Length - lastDotIndex - 1 < 2)
            throw new ArgumentException("Email domain extension is invalid", nameof(email));
        if (email.Length > 254)
            throw new ArgumentException("Email is too long (max 254 characters)", nameof(email));
        if (email.Contains(" "))
            throw new ArgumentException("Email cannot contain spaces", nameof(email));
    }
    private static void ValidatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));
        var allowedChars = "0123456789+- ()";
        if (phoneNumber.Any(c => !allowedChars.Contains(c)))
            throw new ArgumentException("Phone number contains invalid characters", nameof(phoneNumber));
        var digitCount = phoneNumber.Count(char.IsDigit);
        if (digitCount < 10)
            throw new ArgumentException("Phone number must have at least 10 digits", nameof(phoneNumber));
        if (digitCount > 15)
            throw new ArgumentException("Phone number has too many digits (max 15)", nameof(phoneNumber));
        if (phoneNumber.StartsWith("+90") || phoneNumber.StartsWith("0090") || phoneNumber.StartsWith("90"))
        {
            if (digitCount != 12)
                throw new ArgumentException("Turkish phone number must have 10 digits after +90", nameof(phoneNumber));
        }
    }
    private static void ValidateNationalId(string nationalId)
    {
        if (string.IsNullOrWhiteSpace(nationalId))
            throw new ArgumentException("National ID cannot be empty", nameof(nationalId));
        if (nationalId.Length != 11 || !nationalId.All(char.IsDigit))
            throw new ArgumentException("National ID must be 11 digits", nameof(nationalId));
        if (nationalId[0] == '0')
            throw new ArgumentException("National ID cannot start with 0", nameof(nationalId));
        int sum = 0;
        for (int i = 0; i < 10; i++)
        {
            sum += int.Parse(nationalId[i].ToString());
        }
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
        if (!studentNumber.All(c => char.IsLetterOrDigit(c)))
            throw new ArgumentException("Student number must be alphanumeric", nameof(studentNumber));
    }
    public static void ValidateEmployeeNumber(string employeeNumber)
    {
        if (string.IsNullOrWhiteSpace(employeeNumber))
            throw new ArgumentException("Employee number cannot be empty", nameof(employeeNumber));
        if (employeeNumber.Length < 5 || employeeNumber.Length > 10)
            throw new ArgumentException("Employee number must be between 5-10 characters", nameof(employeeNumber));
        if (!employeeNumber.All(c => char.IsLetterOrDigit(c)))
            throw new ArgumentException("Employee number must be alphanumeric", nameof(employeeNumber));
    }
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
        if (!IsValidTurkishNationalId(nationalId))
            throw new ArgumentException(
                "National ID must be 11 digits with valid checksum",
                nameof(nationalId)
            );
        if (!IsValidEmail(email))
            throw new ArgumentException(
                "Email format is invalid",
                nameof(email)
            );
        if (!IsValidPhoneNumber(phoneNumber))
            throw new ArgumentException(
                "Phone number must be Turkish format (e.g., +905XX XXX XXXX or 05XX XXX XXXX)",
                nameof(phoneNumber)
            );
    }
    private static bool IsValidTurkishNationalId(string id)
    {
        if (string.IsNullOrWhiteSpace(id) || id.Length != 11 || !id.All(char.IsDigit))
            return false;
        if (id[0] == '0')
            return false;
        int oddSum = 0;
        for (int i = 0; i < 10; i += 2)
        {
            oddSum += int.Parse(id[i].ToString());
        }
        int evenSum = 0;
        for (int i = 1; i < 10; i += 2)
        {
            evenSum += int.Parse(id[i].ToString());
        }
        int checkSum = ((oddSum * 7) - evenSum) % 11;
        if (checkSum < 0)
            checkSum = 11 + checkSum;
        return checkSum == int.Parse(id[10].ToString());
    }
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    private static bool IsValidPhoneNumber(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;
        var cleaned = Regex.Replace(phone, @"[\s\-]", "");
        var turkishPattern = @"^(\+90|0)[1-9]\d{9}$";
        return Regex.IsMatch(cleaned, turkishPattern);
    }
    private static void ValidateBirthDate(DateTime birthDate)
    {
        if (birthDate > DateTime.UtcNow)
            throw new ArgumentException(
                "Birth date cannot be in the future",
                nameof(birthDate)
            );
        var today = DateTime.UtcNow;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age))
            age--;
        if (age < 16 || age > 100)
            throw new ArgumentException(
                "Person must be between 16 and 100 years old",
                nameof(birthDate)
            );
    }
}