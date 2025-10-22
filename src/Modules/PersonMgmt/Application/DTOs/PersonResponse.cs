namespace PersonMgmt.Application.DTOs;

/// <summary>
/// Kişi bilgilerini response vermek için
/// </summary>
public class PersonResponse
{
    /// <summary>
    /// Kişi ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Ad
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Soyad
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// T.C. Kimlik Numarası
    /// </summary>
    public string NationalId { get; set; }

    /// <summary>
    /// Doğum tarihi
    /// </summary>
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// Cinsiyet
    /// </summary>
    public string Gender { get; set; }

    /// <summary>
    /// E-posta
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Telefon numarası
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Departman ID (opsiyonel)
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// Profil fotoğrafı URL'si (opsiyonel)
    /// </summary>
    public string? ProfilePhotoUrl { get; set; }

    /// <summary>
    /// Oluşturulma tarihi
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Güncellenme tarihi
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Öğrenci mi?
    /// </summary>
    public bool IsStudent { get; set; }

    /// <summary>
    /// Personel mi?
    /// </summary>
    public bool IsStaff { get; set; }

    /// <summary>
    /// Aktif kısıtlama sayısı
    /// </summary>
    public int ActiveRestrictionCount { get; set; }
}