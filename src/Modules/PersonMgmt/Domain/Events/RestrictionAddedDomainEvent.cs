using Core.Domain.Events;

namespace PersonMgmt.Domain.Events;

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