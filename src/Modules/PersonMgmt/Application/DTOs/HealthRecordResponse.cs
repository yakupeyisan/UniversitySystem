namespace PersonMgmt.Application.DTOs;

/// <summary>
/// Sağlık kaydı bilgilerini response vermek için
/// </summary>
public class HealthRecordResponse
{
    /// <summary>
    /// Sağlık kaydı ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Kişi ID
    /// </summary>
    public Guid PersonId { get; set; }

    /// <summary>
    /// Kan grubu
    /// </summary>
    public string? BloodType { get; set; }

    /// <summary>
    /// Alerji bilgisi
    /// </summary>
    public string? Allergies { get; set; }

    /// <summary>
    /// Kronik hastalıklar
    /// </summary>
    public string? ChronicDiseases { get; set; }

    /// <summary>
    /// İlaçlar
    /// </summary>
    public string? Medications { get; set; }

    /// <summary>
    /// Acil durum sağlık bilgisi
    /// </summary>
    public string? EmergencyHealthInfo { get; set; }

    /// <summary>
    /// Son sağlık kontrolü tarihi
    /// </summary>
    public DateTime? LastCheckupDate { get; set; }

    /// <summary>
    /// Notlar
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Oluşturulma tarihi
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Güncellenme tarihi
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}