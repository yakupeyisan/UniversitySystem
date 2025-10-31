using Core.Domain.Events;

namespace Identity.Domain.Events;

public class FailedLoginAttemptEvent : DomainEvent
{
    public FailedLoginAttemptEvent(string email, string ipAddress, string reason)
    {
        Email = email;
        IpAddress = ipAddress;
        Reason = reason;
        OccurredOn = DateTime.UtcNow;
    }

    public string Email { get; }
    public string IpAddress { get; }
    public string Reason { get; }
    public DateTime OccurredOn { get; }
}