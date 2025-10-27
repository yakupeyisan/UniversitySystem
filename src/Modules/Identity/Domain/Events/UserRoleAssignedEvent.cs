using Core.Domain.Events;

namespace Identity.Domain.Events;

public class UserRoleAssignedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public string RoleName { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserRoleAssignedEvent(Guid userId, string email, string roleName)
    {
        UserId = userId;
        Email = email;
        RoleName = roleName;
    }
}