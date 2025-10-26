using Core.Domain;
using Core.Domain.Specifications;
using PersonMgmt.Domain.Enums;
using PersonMgmt.Domain.Events;
namespace PersonMgmt.Domain.Aggregates;
public class Staff : AuditableEntity, ISoftDelete
{
    public string EmployeeNumber { get; private set; }
    public AcademicTitle AcademicTitle { get; private set; }
    public DateTime HireDate { get; private set; }
    public DateTime? TerminationDate { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }
    private Staff()
    {
    }
    public static Staff Create(
    string employeeNumber,
    AcademicTitle academicTitle,
    DateTime hireDate)
    {
        if (string.IsNullOrWhiteSpace(employeeNumber))
            throw new ArgumentException("Employee number cannot be empty", nameof(employeeNumber));
        if (hireDate > DateTime.UtcNow)
            throw new ArgumentException("Hire date cannot be in the future", nameof(hireDate));
        return new Staff
        {
            Id = Guid.NewGuid(),
            EmployeeNumber = employeeNumber.Trim(),
            AcademicTitle = academicTitle,
            HireDate = hireDate,
            TerminationDate = null,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
    public void Activate()
    {
        IsActive = true;
        TerminationDate = null;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Terminate()
    {
        IsActive = false;
        TerminationDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
    public void UpdateAcademicTitle(AcademicTitle newTitle)
    {
        if (AcademicTitle == newTitle)
            return;
        AcademicTitle = newTitle;
        UpdatedAt = DateTime.UtcNow;
    }
    public void PromoteToNextTitle()
    {
        AcademicTitle = AcademicTitle switch
        {
            AcademicTitle.Assistant => AcademicTitle.ResearchAssistant,
            AcademicTitle.ResearchAssistant => AcademicTitle.Lecturer,
            AcademicTitle.Lecturer => AcademicTitle.AssociateProfessor,
            AcademicTitle.AssociateProfessor => AcademicTitle.Professor,
            AcademicTitle.Professor => AcademicTitle.Professor,
            AcademicTitle.Doctor => AcademicTitle.Professor,
            _ => throw new InvalidOperationException($"Unknown academic title: {AcademicTitle}")
        };
        UpdatedAt = DateTime.UtcNow;
    }
    public int YearsOfService
    {
        get
        {
            var endDate = TerminationDate ?? DateTime.UtcNow;
            return (endDate - HireDate).Days / 365;
        }
    }
    public bool IsProfessional =>
    AcademicTitle == AcademicTitle.Professor ||
    AcademicTitle == AcademicTitle.AssociateProfessor;
    public bool IsInManagement =>
    AcademicTitle == AcademicTitle.Professor ||
    AcademicTitle == AcademicTitle.AssociateProfessor;
    public void Terminate(DateTime terminationDate)
    {
        if (terminationDate < HireDate)
            throw new InvalidOperationException(
                $"Termination date ({terminationDate:yyyy-MM-dd}) cannot be before hire date ({HireDate:yyyy-MM-dd})"
            );
        if (IsDeleted)
            throw new InvalidOperationException(
                "Cannot terminate an already deleted staff member"
            );
        if (!IsActive)
            throw new InvalidOperationException(
                "Staff member is already inactive"
            );
        TerminationDate = terminationDate;
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new StaffTerminatedDomainEvent(
            Id,
            EmployeeNumber,
            terminationDate,
            YearsOfService,
            DateTime.UtcNow
        ));
    }
    public void Rehire(DateTime newHireDate)
    {
        if (!IsDeleted && TerminationDate.HasValue)
        {
            TerminationDate = null;
            IsActive = true;
            if (newHireDate < HireDate)
                HireDate = newHireDate;
            UpdatedAt = DateTime.UtcNow;
        }
    }
    public void Delete(Guid deletedBy)
    {
        IsActive = false;
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
    public bool IsCurrentlyEmployed =>
    IsActive &&
    !IsDeleted &&
    (TerminationDate == null || TerminationDate > DateTime.UtcNow);
}