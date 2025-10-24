using Core.Domain.ValueObjects;
namespace PersonMgmt.Domain.ValueObjects;
public class EmergencyContact : ValueObject
{
    public string FullName { get; }
    public string Relationship { get; }
    public string PhoneNumber { get; }
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
        if (phoneNumber.Length < 10)
            throw new ArgumentException("Phone number is invalid", nameof(phoneNumber));
        FullName = fullName;
        Relationship = relationship;
        PhoneNumber = phoneNumber;
    }
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