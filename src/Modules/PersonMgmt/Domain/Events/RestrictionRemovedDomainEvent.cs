using Core.Domain.Events;
namespace PersonMgmt.Domain.Events;
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