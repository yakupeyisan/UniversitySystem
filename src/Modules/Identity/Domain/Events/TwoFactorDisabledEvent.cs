using Core.Domain.Events;
namespace Identity.Domain.Events;
public class TwoFactorDisabledEvent : DomainEvent
{
    public TwoFactorDisabledEvent(Guid userId)
    {
        UserId = userId;
        OccurredOn = DateTime.UtcNow;
    }
    public Guid UserId { get; }
    public DateTime OccurredOn { get; }
}