using Core.Domain;

namespace PersonMgmt.Domain.Aggregates;

public class Address : Entity
{
    public Guid PersonId { get; set; }
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string? PostalCode { get; set; }

    /// <summary>
    /// Adresinim geçerli olduðu baþlangýç tarihi
    /// </summary>
    public DateTime ValidFrom { get; set; }

    /// <summary>
    /// Adresinim geçerli olduðu bitiþ tarihi (NULL = hala geçerli)
    /// </summary>
    public DateTime? ValidTo { get; set; }

    /// <summary>
    /// Bu adres kiþinin þu anki adresi mi?
    /// </summary>
    public bool IsCurrent { get; set; }

    // Soft delete
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public string FullAddress =>
        $"{Street}, {City}, {Country}" +
        (string.IsNullOrEmpty(PostalCode) ? "" : $", {PostalCode}");

    /// <summary>
    /// Adresinin þu anda aktif olup olmadýðýný kontrol et
    /// </summary>
    public bool IsActive =>
        IsCurrent &&
        !IsDeleted &&
        (!ValidTo.HasValue || ValidTo > DateTime.UtcNow);

    /// <summary>
    /// Yeni bir adres oluþtur
    /// </summary>
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

    /// <summary>
    /// Türkiye için adres oluþtur (Country otomatik)
    /// </summary>
    public static Address CreateTurkish(
        Guid personId,
        string street,
        string city,
        string? postalCode = null)
    {
        return Create(personId, street, city, "Turkey", postalCode);
    }

    /// <summary>
    /// Adresini archive et (eski adres olarak iþaretle)
    /// </summary>
    public void Archive()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot archive a deleted address");

        ValidTo = DateTime.UtcNow;
        IsCurrent = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adresini sil (soft delete)
    /// </summary>
    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adresini geri yükle
    /// </summary>
    public void Restore()
    {
        IsDeleted = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adres bilgisini güncelle
    /// </summary>
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