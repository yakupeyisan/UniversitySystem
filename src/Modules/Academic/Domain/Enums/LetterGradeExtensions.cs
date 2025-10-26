namespace Academic.Domain.Enums;

/// <summary>
/// Extension methods for LetterGrade enum
/// </summary>
public static class LetterGradeExtensions
{
    /// <summary>
    /// Convert numeric score to letter grade
    /// </summary>
    public static LetterGrade FromNumericScore(float score)
    {
        return score switch
        {
            >= 90 => LetterGrade.AA,
            >= 85 => LetterGrade.BA,
            >= 80 => LetterGrade.BB,
            >= 75 => LetterGrade.CB,
            >= 70 => LetterGrade.CC,
            >= 65 => LetterGrade.DC,
            >= 60 => LetterGrade.DD,
            > 0 => LetterGrade.F,
            _ => LetterGrade.FF
        };
    }

    /// <summary>
    /// Get grade point for letter grade (4.0 scale)
    /// </summary>
    public static float GetGradePoint(this LetterGrade letterGrade)
    {
        return letterGrade switch
        {
            LetterGrade.AA => 4.0f,
            LetterGrade.BA => 3.7f,
            LetterGrade.BB => 3.3f,
            LetterGrade.CB => 3.0f,
            LetterGrade.CC => 2.7f,
            LetterGrade.DC => 2.3f,
            LetterGrade.DD => 2.0f,
            LetterGrade.F => 0.0f,
            LetterGrade.FF => 0.0f,
            _ => 0.0f
        };
    }

    /// <summary>
    /// Check if letter grade is passing
    /// </summary>
    public static bool IsPassingGrade(this LetterGrade letterGrade)
    {
        return letterGrade is not (LetterGrade.F or LetterGrade.FF);
    }

    /// <summary>
    /// Get numeric score lower bound for letter grade
    /// </summary>
    public static float GetLowerBound(this LetterGrade letterGrade)
    {
        return letterGrade switch
        {
            LetterGrade.AA => 90,
            LetterGrade.BA => 85,
            LetterGrade.BB => 80,
            LetterGrade.CB => 75,
            LetterGrade.CC => 70,
            LetterGrade.DC => 65,
            LetterGrade.DD => 60,
            LetterGrade.F => 0,
            LetterGrade.FF => 0,
            _ => 0
        };
    }
}
