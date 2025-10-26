using Core.Domain;
namespace PersonMgmt.Domain.Aggregates;
public class Address : AuditableEntity
{
    public Guid PersonId { get; set; }
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string? PostalCode { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsDeleted { get; set; }
    public string FullAddress =>
        $"{Street}, {City}, {Country}" +
        (string.IsNullOrEmpty(PostalCode) ? "" : $", {PostalCode}");
    public bool IsActive =>
    IsCurrent &&
    !IsDeleted &&
    (!ValidTo.HasValue || ValidTo > DateTime.UtcNow);
    public static Address Create(
    Guid personId,
    string street,
    string city,
    string country,
    string? postalCode = null)
    {
        ValidateAddress(street, city, country, postalCode);
        return new Address
        {
            PersonId = personId,
            Street = street.Trim(),
            City = city.Trim(),
            Country = country.Trim(),
            PostalCode = string.IsNullOrEmpty(postalCode) ? null : postalCode.Trim(),
            ValidFrom = DateTime.UtcNow,
            IsCurrent = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
    public static Address CreateTurkish(
    Guid personId,
    string street,
    string city,
    string? postalCode = null)
    {
        return Create(personId, street, city, "Turkey", postalCode);
    }
    public void Archive()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot archive a deleted address");
        ValidTo = DateTime.UtcNow;
        IsCurrent = false;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Restore()
    {
        IsDeleted = false;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Update(string street, string city, string country, string? postalCode = null)
    {
        ValidateAddress(street, city, country, postalCode);
        Street = street.Trim();
        City = city.Trim();
        Country = country.Trim();
        PostalCode = string.IsNullOrEmpty(postalCode) ? null : postalCode.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
    private static void ValidateAddress(
        string street,
        string city,
        string country,
        string? postalCode)
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
    }
    public override string ToString() => FullAddress;
}