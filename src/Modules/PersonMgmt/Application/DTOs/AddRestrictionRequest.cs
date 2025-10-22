namespace PersonMgmt.Application.DTOs;

/// <summary>
/// Kişiye kısıtlama eklemek için request
/// </summary>
public class AddRestrictionRequest
{
    /// <summary>
    /// Kısıtlama türü
    /// </summary>
    public byte RestrictionType { get; set; }

    /// <summary>
    /// Kısıtlama seviyesi
    /// </summary>
    public byte RestrictionLevel { get; set; }

    /// <summary>
    /// Başlangıç tarihi
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Bitiş tarihi (opsiyonel)
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Neden
    /// </summary>
    public string Reason { get; set; }

    /// <summary>
    /// Şiddeti (1-10 arası)
    /// </summary>
    public int Severity { get; set; }
}