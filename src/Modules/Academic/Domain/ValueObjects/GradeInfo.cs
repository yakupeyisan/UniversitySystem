using Academic.Domain.Enums;
using Core.Domain.ValueObjects;

namespace Academic.Domain.ValueObjects;

public class GradeInfo : ValueObject
{
    private GradeInfo(float numericScore, LetterGrade letterGrade, float gradePoint)
    {
        NumericScore = numericScore;
        LetterGrade = letterGrade;
        GradePoint = gradePoint;
    }

    public float NumericScore { get; }
    public LetterGrade LetterGrade { get; }
    public float GradePoint { get; }

    public static GradeInfo Create(float numericScore, LetterGrade letterGrade, float gradePoint)
    {
        if (numericScore < 0 || numericScore > 100)
            throw new ArgumentException("Numeric score must be between 0 and 100");
        if (gradePoint < 0 || gradePoint > 4.0)
            throw new ArgumentException("Grade point must be between 0 and 4.0");
        return new GradeInfo(numericScore, letterGrade, gradePoint);
    }

    public bool IsPassingGrade()
    {
        return LetterGrade is not (LetterGrade.F or LetterGrade.FF);
    }

    public bool IsPerfectGrade()
    {
        return LetterGrade == LetterGrade.AA && NumericScore >= 95;
    }

    public override string ToString()
    {
        return $"{LetterGrade} ({NumericScore:F2}) - GPA: {GradePoint:F2}";
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return NumericScore;
        yield return (int)LetterGrade;
        yield return GradePoint;
    }
}