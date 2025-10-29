using Core.Domain;
using Core.Domain.Specifications;
using PersonMgmt.Domain.Enums;

namespace PersonMgmt.Domain.Aggregates;

public class PersonRestriction : AuditableEntity, ISoftDelete
{
    private PersonRestriction()
    {
    }

    public Guid PersonId { get; private set; }
    public RestrictionType RestrictionType { get; private set; }
    public RestrictionLevel RestrictionLevel { get; private set; }
    public Guid AppliedBy { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string Reason { get; private set; }
    public int Severity { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }

    public void Delete(Guid deletedBy)
    {
        IsDeleted = true;
        IsActive = false;
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

    public static PersonRestriction Create(
        Guid personId,
        RestrictionType restrictionType,
        RestrictionLevel restrictionLevel,
        Guid appliedBy,
        DateTime startDate,
        DateTime? endDate,
        string reason,
        int severity)
    {
        ValidateRestrictionData(reason, severity, startDate, endDate);
        return new PersonRestriction
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            RestrictionType = restrictionType,
            RestrictionLevel = restrictionLevel,
            AppliedBy = appliedBy,
            StartDate = startDate,
            EndDate = endDate,
            Reason = reason.Trim(),
            Severity = severity,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public bool IsCurrentlyActive()
    {
        var now = DateTime.UtcNow;
        if (IsDeleted)
            return false;
        if (!IsActive)
            return false;
        if (now < StartDate)
            return false;
        if (EndDate.HasValue && now > EndDate.Value)
            return false;
        return true;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reactivate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ExtendEndDate(DateTime newEndDate)
    {
        if (newEndDate <= EndDate)
            throw new ArgumentException("New end date must be after current end date", nameof(newEndDate));
        EndDate = newEndDate;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateRestrictionData(string reason, int severity, DateTime startDate, DateTime? endDate)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason cannot be empty", nameof(reason));
        if (severity < 0 || severity > 10)
            throw new ArgumentException("Severity must be between 0 and 10", nameof(severity));
        if (startDate > DateTime.UtcNow.AddYears(100))
            throw new ArgumentException("Start date cannot be more than 100 years in the future", nameof(startDate));
        if (endDate.HasValue && endDate.Value < startDate)
            throw new ArgumentException("End date must be after start date", nameof(endDate));
    }
}