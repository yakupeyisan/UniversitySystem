using Core.Domain.ValueObjects;

namespace PersonMgmt.Domain.Aggregates.Person.ValueObjects;

/// <summary>
/// Address - Adres bilgisi Value Object
/// 
/// Özellikleri:
/// - Immutable (değişmez)
/// - Value equality (tüm component'lerine göre eşit olur)
/// - Person'ın başka Aggregate'ine ihtiyacı yok
/// 
/// Örnek kullanım:
/// var address = new Address("Main St. 123", "Istanbul", "Istanbul", "34000", "Turkey");
/// var address2 = new Address("Main St. 123", "Istanbul", "Istanbul", "34000", "Turkey");
/// address == address2 → true
/// </summary>
public class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string PostalCode { get; }
    public string Country { get; }
    public string FullAddress { get; }

    /// <summary>
    /// Private constructor - Factory method'lar aracılığıyla oluştur
    /// </summary>
    private Address(
        string street,
        string city,
        string state,
        string postalCode,
        string country)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty", nameof(street));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty", nameof(country));

        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
        FullAddress = $"{street}, {city}, {state}, {postalCode}, {country}";
    }

    /// <summary>
    /// Factory method - Address oluştur
    /// </summary>
    public static Address Create(
        string street,
        string city,
        string state,
        string postalCode,
        string country)
    {
        return new Address(street, city, state, postalCode, country);
    }

    /// <summary>
    /// Empty address oluştur (nullable için)
    /// </summary>
    public static Address? CreateEmpty() => null;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return PostalCode;
        yield return Country;
    }

    public override string ToString() => FullAddress;
}