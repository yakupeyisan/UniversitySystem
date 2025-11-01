using Core.Domain.Events;
namespace Identity.Domain.Events;
public class RoleRemovedFromUserEvent : DomainEvent
{
    public RoleRemovedFromUserEvent(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
        OccurredOn = DateTime.UtcNow;
    }
    public Guid UserId { get; }
    public Guid RoleId { get; }
    public DateTime OccurredOn { get; }
}