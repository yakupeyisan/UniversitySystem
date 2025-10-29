using MediatR;

namespace Core.Domain.Events;

public abstract class DomainEvent : INotification
{
    protected DomainEvent()
    {
    }

    protected DomainEvent(Guid aggregateId)
    {
        AggregateId = aggregateId;
    }

    public Guid EventId { get; private set; } = Guid.NewGuid();
    public Guid AggregateId { get; protected set; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public int Version { get; set; }

    public override string ToString()
    {
        return $"{GetType().Name} - AggregateId: {AggregateId}, OccurredOn: {OccurredOn:yyyy-MM-dd HH:mm:ss}";
    }
}