namespace Academic.Domain.Enums;

/// <summary>
/// Extension methods for LetterGrade enum
/// </summary>
public static class LetterGradeExtensions
{
    public static float GetGradePoint(this LetterGrade grade) =>
        grade switch
        {
            LetterGrade.AA => 4.0f,
            LetterGrade.BA => 3.5f,
            LetterGrade.BB => 3.0f,
            LetterGrade.CB => 2.5f,
            LetterGrade.CC => 2.0f,
            LetterGrade.DC => 1.5f,
            LetterGrade.DD => 1.0f,
            LetterGrade.F => 0.0f,
            LetterGrade.FF => 0.0f,
            _ => 0.0f
        };

    public static bool IsPassingGrade(this LetterGrade grade) =>
        grade is not (LetterGrade.F or LetterGrade.FF);

    public static LetterGrade FromNumericScore(float score) =>
        score switch
        {
            >= 95 => LetterGrade.AA,
            >= 90 => LetterGrade.BA,
            >= 85 => LetterGrade.BB,
            >= 80 => LetterGrade.CB,
            >= 75 => LetterGrade.CC,
            >= 70 => LetterGrade.DC,
            >= 65 => LetterGrade.DD,
            >= 50 => LetterGrade.F,
            _ => LetterGrade.FF
        };
}