using Core.Domain.ValueObjects;
namespace PersonMgmt.Domain.ValueObjects;
public class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string Country { get; }
    public string? PostalCode { get; }
    public string FullAddress =>
    $"{Street}, {City}, {Country}" +
    (string.IsNullOrEmpty(PostalCode) ? "" : $", {PostalCode}");
    private Address(
    string street,
    string city,
    string country,
    string? postalCode = null)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty", nameof(street));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty", nameof(country));
        if (street.Length < 5)
            throw new ArgumentException("Street must be at least 5 characters", nameof(street));
        if (city.Length < 2)
            throw new ArgumentException("City must be at least 2 characters", nameof(city));
        if (country.Length < 2)
            throw new ArgumentException("Country must be at least 2 characters", nameof(country));
        if (!string.IsNullOrEmpty(postalCode) && postalCode.Length < 4)
            throw new ArgumentException("Postal code must be at least 4 characters", nameof(postalCode));
        Street = street.Trim();
        City = city.Trim();
        Country = country.Trim();
        PostalCode = string.IsNullOrEmpty(postalCode) ? null : postalCode.Trim();
    }
    public static Address Create(
    string street,
    string city,
    string country,
    string? postalCode = null)
    {
        return new Address(street, city, country, postalCode);
    }
    public static Address CreateTurkish(
    string street,
    string city,
    string? postalCode = null)
    {
        return new Address(street, city, "Turkey", postalCode);
    }
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return Country;
        yield return PostalCode;
    }
    public override string ToString() => FullAddress;
}