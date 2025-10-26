// TODO: Define value object

using System.Text.RegularExpressions;
using Core.Domain.ValueObjects;
namespace Academic.Domain.ValueObjects;
/// <summary>
/// ValueObject representing a unique course code (e.g., "CS101")
/// Format: 2-4 uppercase letters followed by 3 digits
/// </summary>
public class CourseCode : ValueObject
{
    public string Value { get; }

    private CourseCode(string value)
    {
        Value = value;
    }

    public static CourseCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Course code cannot be empty");

        value = value.Trim().ToUpper();

        if (!Regex.IsMatch(value, @"^[A-Z]{2,4}\d{3}$"))
            throw new ArgumentException(
                "Invalid course code format. Expected format: 2-4 uppercase letters followed by 3 digits (e.g., CS101, MATH201)");

        return new CourseCode(value);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}