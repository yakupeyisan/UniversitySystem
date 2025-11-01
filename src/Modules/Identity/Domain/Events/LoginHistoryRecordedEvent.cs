using Core.Domain.Events;
using Identity.Domain.Enums;
namespace Identity.Domain.Events;
public class LoginHistoryRecordedEvent : DomainEvent
{
    public LoginHistoryRecordedEvent(Guid userId, LoginResultType result, string ipAddress)
    {
        UserId = userId;
        Result = result;
        IpAddress = ipAddress;
        OccurredOn = DateTime.UtcNow;
    }
    public Guid UserId { get; }
    public LoginResultType Result { get; }
    public string IpAddress { get; }
    public DateTime OccurredOn { get; }
}