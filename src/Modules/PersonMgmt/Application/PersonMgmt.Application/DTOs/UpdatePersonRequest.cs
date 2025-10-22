namespace PersonMgmt.Application.DTOs;

/// <summary>
/// Kişi güncellemek için request
/// </summary>
public class UpdatePersonRequest
{
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