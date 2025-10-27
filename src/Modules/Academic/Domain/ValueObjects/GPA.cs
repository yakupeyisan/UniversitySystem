using Core.Domain.ValueObjects;
namespace Academic.Domain.ValueObjects;
public class GPA : ValueObject
{
    public float SGPA { get; }
    public float CGPA { get; }
    private GPA(float sgpa, float cgpa)
    {
        if (sgpa < 0 || sgpa > 4.0)
            throw new ArgumentException("SGPA must be between 0 and 4.0");
        if (cgpa < 0 || cgpa > 4.0)
            throw new ArgumentException("CGPA must be between 0 and 4.0");
        SGPA = sgpa;
        CGPA = cgpa;
    }
    public static GPA Create(float sgpa, float cgpa)
    {
        return new GPA(sgpa, cgpa);
    }
    public static GPA CreateFromSGPA(float sgpa)
    {
        return new GPA(sgpa, sgpa);
    }
    public bool IsExcellent() => CGPA >= 3.5;
    public bool IsGood() => CGPA >= 3.0 && CGPA < 3.5;
    public bool IsAcceptable() => CGPA >= 2.0 && CGPA < 3.0;
    public bool IsBelow2GPA() => CGPA < 2.0;
    public bool IsGraduationEligible() => CGPA >= 2.0;
    public override string ToString() => $"SGPA: {SGPA:F2}, CGPA: {CGPA:F2}";
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return SGPA;
        yield return CGPA;
    }
}