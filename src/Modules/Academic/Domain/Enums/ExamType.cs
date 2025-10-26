namespace Academic.Domain.Enums;

/// <summary>
/// Enum representing types of exams
/// </summary>
public enum ExamType
{
    /// <summary>
    /// Midterm exam
    /// </summary>
    Midterm = 1,

    /// <summary>
    /// Final exam
    /// </summary>
    Final = 2,

    /// <summary>
    /// Makeup/Resit exam
    /// </summary>
    Makeup = 3,

    /// <summary>
    /// Resit exam (Retake)
    /// </summary>
    Resit = 4,

    /// <summary>
    /// Supplementary exam (Ek sýnav)
    /// </summary>
    Supplementary = 5
}