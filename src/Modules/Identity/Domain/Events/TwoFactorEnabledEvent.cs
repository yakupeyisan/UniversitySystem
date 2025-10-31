using Core.Domain.Events;
using Identity.Domain.Enums;

namespace Identity.Domain.Events;

public class TwoFactorEnabledEvent : DomainEvent
{
    public TwoFactorEnabledEvent(Guid userId, TwoFactorMethod method)
    {
        UserId = userId;
        Method = method;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public TwoFactorMethod Method { get; }
    public DateTime OccurredOn { get; }
}