using System.Text.RegularExpressions;
using Core.Domain.ValueObjects;

namespace Academic.Domain.ValueObjects;

public class CourseCode : ValueObject
{
    private CourseCode(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static CourseCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Course code cannot be empty");
        if (value.Length > 20)
            throw new ArgumentException("Course code cannot exceed 20 characters");
        if (!IsValidFormat(value))
            throw new ArgumentException(
                "Course code format is invalid. Expected format: [LETTERS][NUMBERS] (e.g., CS101)");
        return new CourseCode(value.ToUpper());
    }

    private static bool IsValidFormat(string code)
    {
        return Regex.IsMatch(code, @"^[A-Z]{2,4}\d{2,4}$");
    }

    public override string ToString()
    {
        return Value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}