using Core.Domain.ValueObjects;

namespace PersonMgmt.Domain.ValueObjects;

/// <summary>
/// 🆕 NEW: Address - Adres Value Object
/// 
/// Özellikleri:
/// - Immutable (değişmez)
/// - Value equality
/// - Staff ve Student adreslerini represent eder
/// 
/// Örnek:
/// var address = new Address("123 Main St", "New York", "USA", "10001");
/// </summary>
public class Address : ValueObject
{
    /// <summary>
    /// Cadde / Sokak
    /// </summary>
    public string Street { get; }

    /// <summary>
    /// Şehir
    /// </summary>
    public string City { get; }

    /// <summary>
    /// Ülke
    /// </summary>
    public string Country { get; }

    /// <summary>
    /// Posta kodu (opsiyonel)
    /// </summary>
    public string? PostalCode { get; }

    /// <summary>
    /// Tam adres
    /// </summary>
    public string FullAddress =>
        $"{Street}, {City}, {Country}" +
        (string.IsNullOrEmpty(PostalCode) ? "" : $", {PostalCode}");

    /// <summary>
    /// Private constructor
    /// </summary>
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

        // Postal code length validation
        if (!string.IsNullOrEmpty(postalCode) && postalCode.Length < 4)
            throw new ArgumentException("Postal code must be at least 4 characters", nameof(postalCode));

        Street = street.Trim();
        City = city.Trim();
        Country = country.Trim();
        PostalCode = string.IsNullOrEmpty(postalCode) ? null : postalCode.Trim();
    }

    /// <summary>
    /// Factory method - Address oluştur
    /// </summary>
    public static Address Create(
        string street,
        string city,
        string country,
        string? postalCode = null)
    {
        return new Address(street, city, country, postalCode);
    }

    /// <summary>
    /// Factory method - Türkiye'deki adres oluştur
    /// </summary>
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


