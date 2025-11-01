using Core.Domain.Events;
namespace Identity.Domain.Events;
public class RoleAssignedToUserEvent : DomainEvent
{
    public RoleAssignedToUserEvent(Guid userId, Guid roleId, string roleName)
    {
        UserId = userId;
        RoleId = roleId;
        RoleName = roleName;
        OccurredOn = DateTime.UtcNow;
    }
    public Guid UserId { get; }
    public Guid RoleId { get; }
    public string RoleName { get; }
    public DateTime OccurredOn { get; }
}