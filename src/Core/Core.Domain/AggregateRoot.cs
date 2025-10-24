using Core.Domain.Events;
namespace Core.Domain;
public abstract class AggregateRoot : Entity
{
    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    protected AggregateRoot() : base()
    {
    }
    protected AggregateRoot(Guid id) : base(id)
    {
    }
    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        if (domainEvent == null)
            throw new ArgumentNullException(nameof(domainEvent));
        _domainEvents.Add(domainEvent);
    }
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
    public IReadOnlyCollection<DomainEvent> PopDomainEvents()
    {
        var events = _domainEvents.ToList().AsReadOnly();
        ClearDomainEvents();
        return events;
    }
}