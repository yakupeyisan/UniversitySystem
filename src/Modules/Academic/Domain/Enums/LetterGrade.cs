namespace Academic.Domain.Enums;

/// <summary>
/// Enum representing letter grades with corresponding GPA points
/// </summary>
public enum LetterGrade
{
    /// <summary>
    /// AA - Excellent (4.0)
    /// </summary>
    AA = 1,

    /// <summary>
    /// BA - Very Good (3.5)
    /// </summary>
    BA = 2,

    /// <summary>
    /// BB - Good (3.0)
    /// </summary>
    BB = 3,

    /// <summary>
    /// CB - Satisfactory (2.5)
    /// </summary>
    CB = 4,

    /// <summary>
    /// CC - Fair (2.0)
    /// </summary>
    CC = 5,

    /// <summary>
    /// DC - Passing (1.5)
    /// </summary>
    DC = 6,

    /// <summary>
    /// DD - Barely Passing (1.0)
    /// </summary>
    DD = 7,

    /// <summary>
    /// F - Fail (0.0)
    /// </summary>
    F = 8,

    /// <summary>
    /// FF - Fail (0.0) - Did not attend
    /// </summary>
    FF = 9
}