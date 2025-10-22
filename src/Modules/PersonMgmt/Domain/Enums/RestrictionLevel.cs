namespace PersonMgmt.Domain.Enums;

/// <summary>
/// Kısıtlamanın kapsamı
/// </summary>
public enum RestrictionLevel
{
    /// <summary>
    /// Genel (tüm kampüs)
    /// </summary>
    General = 1,

    /// <summary>
    /// Yemekhanede
    /// </summary>
    Cafeteria = 2,

    /// <summary>
    /// Tüm tesislerde
    /// </summary>
    AllFacilities = 3,

    /// <summary>
    /// Belirli alanlar
    /// </summary>
    Specific = 4
}