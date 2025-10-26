namespace Academic.Domain.Enums;

/// <summary>
/// Enum representing course types/requirements
/// </summary>
public enum CourseType
{
    /// <summary>
    /// Compulsory course (Zorunlu Ders)
    /// </summary>
    Compulsory = 1,

    /// <summary>
    /// Elective course (Seçmeli Ders)
    /// </summary>
    Elective = 2,

    /// <summary>
    /// Free elective (Serbest Seçmeli)
    /// </summary>
    FreeElective = 3,

    /// <summary>
    /// Practicum/Lab (Uygulama)
    /// </summary>
    Practicum = 4,

    /// <summary>
    /// Seminar
    /// </summary>
    Seminar = 5
}