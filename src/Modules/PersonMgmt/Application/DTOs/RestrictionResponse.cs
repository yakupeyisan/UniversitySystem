namespace PersonMgmt.Application.DTOs;

/// <summary>
/// Kişi kısıtlama bilgilerini response vermek için
/// </summary>
public class RestrictionResponse
{
    /// <summary>
    /// Kısıtlama ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Kişi ID
    /// </summary>
    public Guid PersonId { get; set; }

    /// <summary>
    /// Kısıtlama türü
    /// </summary>
    public string RestrictionType { get; set; } = string.Empty;

    /// <summary>
    /// Kısıtlama seviyesi
    /// </summary>
    public string RestrictionLevel { get; set; } = string.Empty;

    /// <summary>
    /// Başlangıç tarihi
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Bitiş tarihi
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Kısıtlama nedeni
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Kısıtlama şiddeti (1-10)
    /// </summary>
    public int Severity { get; set; }

    /// <summary>
    /// Aktif mi?
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Kısıtlamayı uygulayan (Admin/User ID)
    /// </summary>
    public Guid AppliedBy { get; set; }

    /// <summary>
    /// Oluşturulma tarihi
    /// </summary>
    public DateTime CreatedAt { get; set; }
}