using Core.Domain;
using Core.Domain.Specifications;

namespace PersonMgmt.Domain.Aggregates;
public class EmergencyContact : AuditableEntity, ISoftDelete
{
    public Guid PersonId { get; set; }
    public string FullName { get; set; } = null!;
    public string Relationship { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public bool IsCurrent { get; set; }
    public int Priority { get; set; } = 1;

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }
    public string ContactInfo => $"{FullName} ({Relationship}) - {PhoneNumber}";
    public bool IsActive =>
    IsCurrent &&
    !IsDeleted &&
    (!ValidTo.HasValue || ValidTo > DateTime.UtcNow);
    public static EmergencyContact Create(
    Guid personId,
    string fullName,
    string relationship,
    string phoneNumber,
    int priority = 1)
    {
        ValidateContact(fullName, relationship, phoneNumber);
        return new EmergencyContact
        {
            PersonId = personId,
            FullName = fullName.Trim(),
            Relationship = relationship.Trim(),
            PhoneNumber = phoneNumber.Trim(),
            Priority = priority,
            ValidFrom = DateTime.UtcNow,
            IsCurrent = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
    public void Archive()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot archive a deleted contact");
        ValidTo = DateTime.UtcNow;
        IsCurrent = false;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Delete(Guid deletedBy)
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        UpdatedBy = deletedBy;
    }
    public void Restore()
    {
        IsDeleted = false;
        DeletedBy = null;
        DeletedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Update(
    string fullName,
    string relationship,
    string phoneNumber,
    int priority = 1)
    {
        ValidateContact(fullName, relationship, phoneNumber);
        FullName = fullName.Trim();
        Relationship = relationship.Trim();
        PhoneNumber = phoneNumber.Trim();
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }
    public void UpdatePriority(int priority)
    {
        if (priority < 1 || priority > 10)
            throw new ArgumentException("Priority must be between 1 and 10", nameof(priority));
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }
    private static void ValidateContact(
        string fullName,
        string relationship,
        string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty", nameof(fullName));
        if (string.IsNullOrWhiteSpace(relationship))
            throw new ArgumentException("Relationship cannot be empty", nameof(relationship));
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));
        if (fullName.Length < 2)
            throw new ArgumentException("Full name must be at least 2 characters", nameof(fullName));
        if (relationship.Length < 2)
            throw new ArgumentException("Relationship must be at least 2 characters", nameof(relationship));
        if (phoneNumber.Length < 10)
            throw new ArgumentException("Phone number is invalid", nameof(phoneNumber));
    }
    public override string ToString() => ContactInfo;
}