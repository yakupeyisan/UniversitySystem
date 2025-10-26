using Core.Domain;
using Core.Domain.Specifications;
using PersonMgmt.Domain.Enums;
namespace PersonMgmt.Domain.Aggregates;
public class Student : AuditableEntity, ISoftDelete
{
    public string StudentNumber { get; private set; }
    public EducationLevel EducationLevel { get; private set; }
    public int CurrentSemester { get; private set; }
    public StudentStatus Status { get; private set; }
    public double CGPA { get; private set; }
    public double SGPA { get; private set; }
    public int TotalCredits { get; private set; }
    public int CompletedCredits { get; private set; }
    public DateTime EnrollmentDate { get; private set; }
    public DateTime? GraduationDate { get; private set; }
    public Guid? AdvisorId { get; private set; }
    public Guid? ProgramId { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }
    private Student()
    {
    }
    public static Student Create(
    string studentNumber,
    EducationLevel educationLevel,
    DateTime enrollmentDate,
    Guid? advisorId = null,
    Guid? programId = null)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
            throw new ArgumentException("Student number cannot be empty", nameof(studentNumber));
        if (enrollmentDate > DateTime.UtcNow)
            throw new ArgumentException("Enrollment date cannot be in the future", nameof(enrollmentDate));
        return new Student
        {
            Id = Guid.NewGuid(),
            StudentNumber = studentNumber.Trim(),
            EducationLevel = educationLevel,
            CurrentSemester = 1,
            Status = StudentStatus.Active,
            CGPA = 0.0,
            SGPA = 0.0,
            TotalCredits = 0,
            CompletedCredits = 0,
            EnrollmentDate = enrollmentDate,
            GraduationDate = null,
            AdvisorId = advisorId,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ProgramId = programId
        };
    }
    public void UpdateStatus(StudentStatus newStatus)
    {
        if (Status == StudentStatus.Graduated && newStatus != StudentStatus.Graduated)
            throw new InvalidOperationException("Graduated student cannot change status");
        if (Status == StudentStatus.Expelled)
            throw new InvalidOperationException("Expelled student cannot change status");
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
        if (newStatus == StudentStatus.Graduated && !GraduationDate.HasValue)
            GraduationDate = DateTime.UtcNow;
    }
    public void Suspend()
    {
        UpdateStatus(StudentStatus.Suspended);
    }
    public void MakePassive()
    {
        UpdateStatus(StudentStatus.Passive);
    }
    public void MakeActive()
    {
        UpdateStatus(StudentStatus.Active);
    }
    public void Graduate()
    {
        UpdateStatus(StudentStatus.Graduated);
    }
    public void Expel()
    {
        UpdateStatus(StudentStatus.Expelled);
    }
    public void UpdateCGPA(double cgpa)
    {
        if (cgpa < 0.0 || cgpa > 4.0)
            throw new ArgumentException("CGPA must be between 0.0 and 4.0", nameof(cgpa));
        CGPA = Math.Round(cgpa, 2);
        UpdatedAt = DateTime.UtcNow;
    }
    public void UpdateSGPA(double sgpa)
    {
        if (sgpa < 0.0 || sgpa > 4.0)
            throw new ArgumentException("SGPA must be between 0.0 and 4.0", nameof(sgpa));
        SGPA = Math.Round(sgpa, 2);
        UpdatedAt = DateTime.UtcNow;
    }
    public void UpdateCredits(int totalCredits, int completedCredits)
    {
        if (totalCredits < 0)
            throw new ArgumentException("Total credits cannot be negative", nameof(totalCredits));
        if (completedCredits < 0)
            throw new ArgumentException("Completed credits cannot be negative", nameof(completedCredits));
        if (completedCredits > totalCredits)
            throw new ArgumentException("Completed credits cannot exceed total credits", nameof(completedCredits));
        TotalCredits = totalCredits;
        CompletedCredits = completedCredits;
        UpdatedAt = DateTime.UtcNow;
    }
    public void IncrementSemester()
    {
        CurrentSemester++;
        UpdatedAt = DateTime.UtcNow;
    }
    public void AssignAdvisor(Guid advisorId)
    {
        if (advisorId == Guid.Empty)
            throw new ArgumentException("Advisor ID cannot be empty", nameof(advisorId));
        AdvisorId = advisorId;
        UpdatedAt = DateTime.UtcNow;
    }
    public void ChangeAdvisor(Guid newAdvisorId)
    {
        if (newAdvisorId == Guid.Empty)
            throw new ArgumentException("New advisor ID cannot be empty", nameof(newAdvisorId));
        AdvisorId = newAdvisorId;
        UpdatedAt = DateTime.UtcNow;
    }
    public void RemoveAdvisor()
    {
        AdvisorId = null;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Delete(Guid deletedBy)
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        UpdatedBy = deletedBy;
    }
    public void Restore()
    {
        IsDeleted = false;
        DeletedBy = null;
        DeletedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }
    public bool HasPassingGPA => CGPA >= 2.0;
    public bool IsGraduated => Status == StudentStatus.Graduated;
    public bool IsCurrentlyActive => Status == StudentStatus.Active && !IsDeleted;
    public double CompletionPercentage => TotalCredits > 0
    ? Math.Round((double)CompletedCredits / TotalCredits * 100, 2)
    : 0.0;
}