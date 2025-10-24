using Core.Domain;
using Core.Domain.Specifications;
using PersonMgmt.Domain.Enums;
namespace PersonMgmt.Domain.Aggregates;
public class PersonRestriction : Entity, ISoftDelete
{
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
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    private PersonRestriction()
    {
    }
    public static PersonRestriction Create(
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
    public void Delete(Guid deletedBy)
    {
        IsDeleted = true;
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
    public void Restore()
    {
        IsDeleted = false;
        DeletedBy = null;
        DeletedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }
    public int? RemainingDays
    {
        get
        {
            if (!EndDate.HasValue || IsDeleted)
                return null;
            var now = DateTime.UtcNow;
            if (now > EndDate.Value)
                return 0;
            return (int)(EndDate.Value - now).TotalDays;
        }
    }
    public bool IsExpired
    {
        get
        {
            if (!EndDate.HasValue)
                return false;
            return DateTime.UtcNow > EndDate.Value;
        }
    }
    public bool IsPermanent => !EndDate.HasValue;
    private static void ValidateRestrictionData(
        string reason,
        int severity,
        DateTime startDate,
        DateTime? endDate)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Restriction reason cannot be empty", nameof(reason));
        if (reason.Length < 10)
            throw new ArgumentException("Restriction reason must be at least 10 characters", nameof(reason));
        if (severity < 1 || severity > 10)
            throw new ArgumentException("Severity must be between 1 and 10", nameof(severity));
        if (startDate > DateTime.UtcNow)
            throw new ArgumentException("Start date cannot be in the future", nameof(startDate));
        if (endDate.HasValue && endDate.Value <= startDate)
            throw new ArgumentException("End date must be after start date", nameof(endDate));
    }
}