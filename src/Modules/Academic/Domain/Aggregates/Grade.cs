using Academic.Domain.Enums;
using Academic.Domain.Events;
using Core.Domain;
using Core.Domain.Specifications;

namespace Academic.Domain.Aggregates;

public class Grade : AuditableEntity, ISoftDelete
{
    private Grade()
    {
    }

    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public Guid RegistrationId { get; private set; }
    public string Semester { get; private set; } = null!;
    public float MidtermScore { get; private set; }
    public float FinalScore { get; private set; }
    public float NumericScore { get; private set; }
    public LetterGrade LetterGrade { get; private set; }
    public float GradePoint { get; private set; }
    public int ECTS { get; private set; }
    public bool IsObjected { get; private set; }
    public DateTime ObjectionDeadline { get; private set; }
    public DateTime RecordedDate { get; private set; }
    public Course? Course { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }

    public void Delete(Guid deletedBy)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Grade is already deleted");
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deletedBy;
    }

    public void Restore()
    {
        if (!IsDeleted)
            throw new InvalidOperationException("Grade is not deleted");
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Grade Create(
        Guid studentId,
        Guid courseId,
        Guid registrationId,
        string semester,
        float midtermScore,
        float finalScore,
        float midtermWeight = 0.3f,
        float finalWeight = 0.7f,
        int ects = 0)
    {
        if (studentId == Guid.Empty)
            throw new ArgumentException("Student ID cannot be empty");
        if (courseId == Guid.Empty)
            throw new ArgumentException("Course ID cannot be empty");
        if (registrationId == Guid.Empty)
            throw new ArgumentException("Registration ID cannot be empty");
        if (string.IsNullOrWhiteSpace(semester))
            throw new ArgumentException("Semester cannot be empty");
        if (midtermScore < 0 || midtermScore > 100)
            throw new ArgumentException("Midterm score must be between 0 and 100");
        if (finalScore < 0 || finalScore > 100)
            throw new ArgumentException("Final score must be between 0 and 100");
        if (midtermWeight < 0 || midtermWeight > 1)
            throw new ArgumentException("Midterm weight must be between 0 and 1");
        if (finalWeight < 0 || finalWeight > 1)
            throw new ArgumentException("Final weight must be between 0 and 1");
        if (Math.Abs(midtermWeight + finalWeight - 1.0f) > 0.001)
            throw new ArgumentException("Sum of weights must equal 1.0");
        var numericScore = midtermScore * midtermWeight + finalScore * finalWeight;
        var letterGrade = LetterGradeExtensions.FromNumericScore(numericScore);
        var gradePoint = letterGrade.GetGradePoint();
        var grade = new Grade
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            CourseId = courseId,
            RegistrationId = registrationId,
            Semester = semester,
            MidtermScore = midtermScore,
            FinalScore = finalScore,
            NumericScore = numericScore,
            LetterGrade = letterGrade,
            GradePoint = gradePoint,
            ECTS = ects,
            IsObjected = false,
            ObjectionDeadline = DateTime.UtcNow.AddDays(14),
            RecordedDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        grade.AddDomainEvent(new GradeRecorded(
            grade.Id,
            studentId,
            courseId,
            letterGrade,
            numericScore));
        return grade;
    }

    public void UpdateScores(
        float newMidtermScore,
        float newFinalScore,
        float midtermWeight = 0.3f,
        float finalWeight = 0.7f)
    {
        if (newMidtermScore < 0 || newMidtermScore > 100)
            throw new ArgumentException("Midterm score must be between 0 and 100");
        if (newFinalScore < 0 || newFinalScore > 100)
            throw new ArgumentException("Final score must be between 0 and 100");
        MidtermScore = newMidtermScore;
        FinalScore = newFinalScore;
        NumericScore = newMidtermScore * midtermWeight + newFinalScore * finalWeight;
        LetterGrade = LetterGradeExtensions.FromNumericScore(NumericScore);
        GradePoint = LetterGrade.GetGradePoint();
        UpdatedAt = DateTime.UtcNow;
    }

    public bool CanObjectGrade()
    {
        return DateTime.UtcNow <= ObjectionDeadline && !IsObjected;
    }

    public void MarkAsObjected()
    {
        if (!CanObjectGrade())
            throw new InvalidOperationException("Grade objection deadline has passed");
        IsObjected = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsPassingGrade()
    {
        return LetterGrade.IsPassingGrade();
    }

    public float GetECTSPoints()
    {
        return ECTS * GradePoint;
    }

    public void UpdateGradeFromObjection(float newMidtermScore, float newFinalScore, LetterGrade newLetterGrade)
    {
        MidtermScore = newMidtermScore;
        FinalScore = newFinalScore;
        NumericScore = newMidtermScore * 0.3f + newFinalScore * 0.7f;
        LetterGrade = newLetterGrade;
        GradePoint = newLetterGrade.GetGradePoint();
        UpdatedAt = DateTime.UtcNow;
    }

    public override string ToString()
    {
        return $"{Semester} - {LetterGrade} ({NumericScore:F2})";
    }
}