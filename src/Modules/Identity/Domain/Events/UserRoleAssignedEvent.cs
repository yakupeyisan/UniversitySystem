using Core.Domain.Events;
namespace Identity.Domain.Events;
public class UserRoleAssignedEvent : DomainEvent
{
    public UserRoleAssignedEvent(Guid userId, string email, string roleName)
    {
        UserId = userId;
        Email = email;
        RoleName = roleName;
    }
    public Guid UserId { get; }
    public string Email { get; }
    public string RoleName { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}