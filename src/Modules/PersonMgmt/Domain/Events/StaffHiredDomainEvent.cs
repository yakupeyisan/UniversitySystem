using Core.Domain.Events;

namespace PersonMgmt.Domain.Events;

/// <summary>
/// Personel işe alındığında raise edilen event
/// </summary>
public class StaffHiredDomainEvent : DomainEvent
{
    public Guid PersonId { get; set; }
    public string EmployeeNumber { get; set; }
    public int AcademicTitle { get; set; }
    public DateTime HireDate { get; set; }

    public StaffHiredDomainEvent(
        Guid personId,
        string employeeNumber,
        int academicTitle,
        DateTime hireDate) : base(personId)  // ✅ FIX: AggregateId set
    {
        PersonId = personId;
        EmployeeNumber = employeeNumber;
        AcademicTitle = academicTitle;
        HireDate = hireDate;
    }
}

/// <summary>
/// 🆕 NEW: RestrictionAddedDomainEvent
/// Kişiye kısıtlama eklendiğinde raise edilen event
/// </summary>
public class RestrictionAddedDomainEvent : DomainEvent
{
    public Guid PersonId { get; set; }
    public Guid RestrictionId { get; set; }
    public string RestrictionType { get; set; }
    public string Reason { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public RestrictionAddedDomainEvent(
        Guid personId,
        Guid restrictionId,
        string restrictionType,
        string reason,
        DateTime startDate,
        DateTime? endDate) : base(personId) 
    {
        PersonId = personId;
        RestrictionId = restrictionId;
        RestrictionType = restrictionType;
        Reason = reason;
        StartDate = startDate;
        EndDate = endDate;
    }
}

/// <summary>
/// 🆕 NEW: RestrictionRemovedDomainEvent
/// Kişiden kısıtlama kaldırıldığında raise edilen event
/// </summary>
public class RestrictionRemovedDomainEvent : DomainEvent
{
    public Guid PersonId { get; set; }
    public Guid RestrictionId { get; set; }
    public string RestrictionType { get; set; }
    public DateTime RemovedAt { get; set; }

    public RestrictionRemovedDomainEvent(
        Guid personId,
        Guid restrictionId,
        string restrictionType,
        DateTime removedAt) : base(personId)  
    {
        PersonId = personId;
        RestrictionId = restrictionId;
        RestrictionType = restrictionType;
        RemovedAt = removedAt;
    }
}


