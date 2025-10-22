namespace PersonMgmt.Application.DTOs;

/// <summary>
/// Kişi oluşturmak için request
/// </summary>
public class CreatePersonRequest
{
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
    /// Cinsiyet (0=Erkek, 1=Kadın)
    /// </summary>
    public byte Gender { get; set; }

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
}