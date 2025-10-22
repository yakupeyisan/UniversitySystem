namespace PersonMgmt.Domain.Enums;

/// <summary>
/// Öğrenci durumu
/// </summary>
public enum StudentStatus
{
    /// <summary>
    /// Aktif öğrenci
    /// </summary>
    Active = 1,

    /// <summary>
    /// Pasif öğrenci
    /// </summary>
    Passive = 2,

    /// <summary>
    /// Öğrenime ara vermiş
    /// </summary>
    Suspended = 3,

    /// <summary>
    /// Mezun
    /// </summary>
    Graduated = 4,

    /// <summary>
    /// Çıkarılan
    /// </summary>
    Expelled = 5
}