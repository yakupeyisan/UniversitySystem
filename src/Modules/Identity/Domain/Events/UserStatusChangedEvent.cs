using Core.Domain.Events;

namespace Identity.Domain.Events;

public class UserStatusChangedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public int Status { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserStatusChangedEvent(Guid userId, string email, int status)
    {
        UserId = userId;
        Email = email;
        Status = status;
    }
}