namespace Academic.Domain.Enums;

/// <summary>
/// Enum representing letter grades with corresponding GPA points
/// </summary>
public enum LetterGrade
{
    /// <summary>
    /// AA - Excellent (90-100)
    /// </summary>
    AA = 1,

    /// <summary>
    /// BA - Very Good (85-89)
    /// </summary>
    BA = 2,

    /// <summary>
    /// BB - Good (80-84)
    /// </summary>
    BB = 3,

    /// <summary>
    /// CB - Satisfactory (75-79)
    /// </summary>
    CB = 4,

    /// <summary>
    /// CC - Sufficient (70-74)
    /// </summary>
    CC = 5,

    /// <summary>
    /// DC - Barely Passing (65-69)
    /// </summary>
    DC = 6,

    /// <summary>
    /// DD - Minimal (60-64)
    /// </summary>
    DD = 7,

    /// <summary>
    /// F - Failing (0-59)
    /// </summary>
    F = 8,

    /// <summary>
    /// FF - Not Attempted
    /// </summary>
    FF = 9
}