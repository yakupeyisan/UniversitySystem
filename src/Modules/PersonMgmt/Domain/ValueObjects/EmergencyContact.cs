using Core.Domain.ValueObjects;

namespace PersonMgmt.Domain.Aggregates.Person.ValueObjects;

/// <summary>
/// EmergencyContact - Acil durum iletişim bilgisi Value Object
/// 
/// Özellikleri:
/// - Immutable (değişmez)
/// - Value equality
/// - Person'a ait
/// 
/// Örnek kullanım:
/// var contact = new EmergencyContact("John Doe", "Father", "555-1234");
/// </summary>
public class EmergencyContact : ValueObject
{
    public string FullName { get; }
    public string Relationship { get; }
    public string PhoneNumber { get; }

    /// <summary>
    /// Private constructor
    /// </summary>
    private EmergencyContact(
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

        // Basic phone validation (optional)
        if (phoneNumber.Length < 10)
            throw new ArgumentException("Phone number is invalid", nameof(phoneNumber));

        FullName = fullName;
        Relationship = relationship;
        PhoneNumber = phoneNumber;
    }

    /// <summary>
    /// Factory method - EmergencyContact oluştur
    /// </summary>
    public static EmergencyContact Create(
        string fullName,
        string relationship,
        string phoneNumber)
    {
        return new EmergencyContact(fullName, relationship, phoneNumber);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FullName;
        yield return Relationship;
        yield return PhoneNumber;
    }

    public override string ToString() => $"{FullName} ({Relationship}) - {PhoneNumber}";
}