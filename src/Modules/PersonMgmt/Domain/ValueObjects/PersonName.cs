using Core.Domain.ValueObjects;

namespace PersonMgmt.Domain.Aggregates.Person.ValueObjects;

/// <summary>
/// PersonName - Ad ve soyad Value Object
/// 
/// Özellikleri:
/// - Immutable
/// - Value equality
/// - Name validations'ını encapsulate eder
/// 
/// Örnek:
/// var name = new PersonName("John", "Doe");
/// </summary>
public class PersonName : ValueObject
{
    /// <summary>
    /// Ad
    /// </summary>
    public string FirstName { get; }

    /// <summary>
    /// Soyad
    /// </summary>
    public string LastName { get; }

    /// <summary>
    /// Tam ad
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Private constructor
    /// </summary>
    private PersonName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        if (firstName.Length < 2)
            throw new ArgumentException("First name must be at least 2 characters", nameof(firstName));
        if (lastName.Length < 2)
            throw new ArgumentException("Last name must be at least 2 characters", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    /// <summary>
    /// Factory method
    /// </summary>
    public static PersonName Create(string firstName, string lastName)
    {
        return new PersonName(firstName, lastName);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }

    public override string ToString() => FullName;
}