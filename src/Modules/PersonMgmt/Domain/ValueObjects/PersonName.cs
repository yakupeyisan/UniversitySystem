using Core.Domain.ValueObjects;
namespace PersonMgmt.Domain.ValueObjects;
public class PersonName : ValueObject
{
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
    public string FirstName { get; }
    public string LastName { get; }
    public string FullName => $"{FirstName} {LastName}";
    public static PersonName Create(string firstName, string lastName)
    {
        return new PersonName(firstName, lastName);
    }
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }
    public override string ToString()
    {
        return FullName;
    }
}