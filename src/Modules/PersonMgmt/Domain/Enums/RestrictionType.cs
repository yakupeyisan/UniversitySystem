namespace PersonMgmt.Domain.Enums;

/// <summary>
/// Kişi kısıtlaması türü
/// (Üniversite içi disiplin, sağlık, mali sorunlar, vb.)
/// </summary>
public enum RestrictionType
{
    /// <summary>
    /// Askıya alma
    /// </summary>
    Suspended = 1,

    /// <summary>
    /// Yasaklama (tüm kampüs)
    /// </summary>
    Banned = 2,

    /// <summary>
    /// COVID karantinası
    /// </summary>
    CovidQuarantine = 3,

    /// <summary>
    /// Ceza yükümlülüğü
    /// </summary>
    CriminalRecord = 4,

    /// <summary>
    /// Sağlık nedeni
    /// </summary>
    MedicalReason = 5,

    /// <summary>
    /// Mali nedeni (borç, vb.)
    /// </summary>
    Financial = 6,

    /// <summary>
    /// Diğer
    /// </summary>
    Other = 7
}