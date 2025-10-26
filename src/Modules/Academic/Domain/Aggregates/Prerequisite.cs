using Academic.Domain.Enums;
using Core.Domain;

namespace Academic.Domain.Aggregates;

/// <summary>
/// Prerequisite entity representing a prerequisite requirement for a course
/// </summary>
public class Prerequisite : AuditableEntity
{
    public Guid CourseId { get; private set; }
    public Guid PrerequisiteCourseId { get; private set; }
    public LetterGrade MinimumGrade { get; private set; }
    public bool IsRequired { get; private set; }
    public bool WaiverAllowed { get; private set; }

    private Prerequisite() { }

    public static Prerequisite Create(
        Guid courseId,
        Guid prerequisiteCourseId,
        LetterGrade minimumGrade = LetterGrade.DD,
        bool isRequired = true,
        bool waiverAllowed = true)
    {
        if (courseId == prerequisiteCourseId)
            throw new ArgumentException("A course cannot be its own prerequisite");

        var prerequisite = new Prerequisite
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            PrerequisiteCourseId = prerequisiteCourseId,
            MinimumGrade = minimumGrade,
            IsRequired = isRequired,
            WaiverAllowed = waiverAllowed,
            CreatedAt = DateTime.UtcNow
        };

        return prerequisite;
    }

    public bool IsSatisfiedByGrade(LetterGrade studentGrade)
    {
        var studentGradePoint = studentGrade.GetGradePoint();
        var minimumGradePoint = MinimumGrade.GetGradePoint();

        return studentGradePoint >= minimumGradePoint;
    }

    public void UpdateMinimumGrade(LetterGrade newMinimumGrade)
    {
        MinimumGrade = newMinimumGrade;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AllowWaiver()
    {
        WaiverAllowed = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DisallowWaiver()
    {
        WaiverAllowed = false;
        UpdatedAt = DateTime.UtcNow;
    }
}