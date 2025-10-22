using MediatR;

namespace Core.Domain.Events;

/// <summary>
/// DomainEvent - Business significant events
/// 
/// Domain events temsil eder:
/// - "Müşteri oluşturuldu"
/// - "Ödeme işlendi"
/// - "Sipariş iptal edildi"
/// 
/// Özellikler:
/// - Immutable (salt okunur)
/// - Self-contained (tüm gerekli bilgiyi içerir)
/// - Published to all subscribers
/// - Can be used for audit trail
/// - Can trigger other domain operations
/// 
/// Süreç:
/// 1. Aggregate root'da business logic'ten sonra raise edilir
/// 2. Repository tarafından persisted edilir
/// 3. Event handlers tarafından subscribe edilir
/// 4. Handler'lar başka business logic'i trigger edebilir
/// </summary>
public abstract class DomainEvent : INotification
{
    /// <summary>
    /// Event'in unique identifier'ı
    /// </summary>
    public Guid EventId { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// Aggregate root'un ID'si (hangi aggregate'e ait)
    /// </summary>
    public Guid AggregateId { get; protected set; }

    /// <summary>
    /// Event oluşturulma zamanı (UTC)
    /// </summary>
    public DateTime OccurredOn { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Event'in aggregate'deki versiyon numarası
    /// (Event sourcing için)
    /// </summary>
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