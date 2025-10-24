using MediatR;
namespace Core.Domain.Events;
public abstract class DomainEvent : INotification
{
    public Guid EventId { get; private set; } = Guid.NewGuid();
    public Guid AggregateId { get; protected set; }
    public DateTime OccurredOn { get; private set; } = DateTime.UtcNow;
    public int Version { get; set; }
    protected DomainEvent()
    {
    }
    protected DomainEvent(Guid aggregateId)
    {
        AggregateId = aggregateId;
    }
    public override string ToString()
    {
        return $"{GetType().Name} - AggregateId: {AggregateId}, OccurredOn: {OccurredOn:yyyy-MM-dd HH:mm:ss}";
    }
}