using Core.Domain;

namespace PersonMgmt.Domain.Aggregates;

/// <summary>
/// EmergencyContact Entity - Bir kiþinin acil durum iletiþim bilgilerini tutar
/// Birden çok contact olabilir (ana, yardýmcý, ebeveyn, vb.)
/// </summary>
public class EmergencyContact : Entity
{
    public Guid PersonId { get; set; }
    public string FullName { get; set; } = null!;
    public string Relationship { get; set; } = null!;  // "Anne", "Baba", "Kardeþ", "Arkadaþ", etc.
    public string PhoneNumber { get; set; } = null!;

    /// <summary>
    /// Bu contact'ýn geçerli olduðu baþlangýç tarihi
    /// </summary>
    public DateTime ValidFrom { get; set; }

    /// <summary>
    /// Bu contact'ýn geçerli olduðu bitiþ tarihi (NULL = hala geçerli)
    /// </summary>
    public DateTime? ValidTo { get; set; }

    /// <summary>
    /// Bu contact þu anda aktif mi?
    /// </summary>
    public bool IsCurrent { get; set; }

    /// <summary>
    /// Contact bilgisinin hangisi prioritesi var (1=en yüksek, 10=en düþük)
    /// </summary>
    public int Priority { get; set; } = 1;

    // Soft delete
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public string ContactInfo => $"{FullName} ({Relationship}) - {PhoneNumber}";

    /// <summary>
    /// Contact'ýn þu anda aktif olup olmadýðýný kontrol et
    /// </summary>
    public bool IsActive =>
        IsCurrent &&
        !IsDeleted &&
        (!ValidTo.HasValue || ValidTo > DateTime.UtcNow);

    /// <summary>
    /// Yeni bir emergency contact oluþtur
    /// </summary>
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

    /// <summary>
    /// Contact'ý archive et (eski contact olarak iþaretle)
    /// </summary>
    public void Archive()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot archive a deleted contact");

        ValidTo = DateTime.UtcNow;
        IsCurrent = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Contact'ý sil (soft delete)
    /// </summary>
    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Contact'ý geri yükle
    /// </summary>
    public void Restore()
    {
        IsDeleted = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Contact bilgisini güncelle
    /// </summary>
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

    /// <summary>
    /// Priority'yi güncelle
    /// </summary>
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