using Core.Domain.Events;
namespace PersonMgmt.Domain.Events;
public class RestrictionAddedDomainEvent : DomainEvent
{
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
    public Guid PersonId { get; set; }
    public Guid RestrictionId { get; set; }
    public string RestrictionType { get; set; }
    public string Reason { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}