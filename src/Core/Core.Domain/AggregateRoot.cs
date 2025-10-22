using Core.Domain.Events;

namespace Core.Domain;

/// <summary>
/// AggregateRoot - Domain-Driven Design'in merkezi
/// 
/// Aggregate Root özellikleri:
/// - Aggregate'in entry point'i
/// - Domain events'i yönetir
/// - Consistency boundary'i temsil eder
/// - Child entities'leri kontrol eder
/// 
/// Sorumluluğu:
/// - Domain events raise ve maintain etmek
/// - Child entities'ler üzerinde business logic enforce etmek
/// - Aggregate'in consistency'sini sağlamak
/// </summary>
public abstract class AggregateRoot : Entity
{
    /// <summary>
    /// Domain events listesi
    /// </summary>
    private readonly List<DomainEvent> _domainEvents = new();

    /// <summary>
    /// Read-only domain events collection
    /// </summary>
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot() : base()
    {
    }

    protected AggregateRoot(Guid id) : base(id)
    {
    }

    /// <summary>
    /// Domain event ekle (protected - sadece aggregate içinden çağrılır)
    /// </summary>
    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        if (domainEvent == null)
            throw new ArgumentNullException(nameof(domainEvent));

        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Tüm domain events'i sil
    /// (Repository tarafından event'ler persisted edildikten sonra çağrılır)
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Domain events'leri al ve temizle
    /// </summary>
    public IReadOnlyCollection<DomainEvent> PopDomainEvents()
    {
        var events = _domainEvents.ToList().AsReadOnly();
        ClearDomainEvents();
        return events;
    }
}