namespace Core.Domain.Repositories;

/// <summary>
/// IGenericRepository - Generic repository interface
/// 
/// Tüm aggregate'ler için ortak repository interface
/// (Person, Payment, Student, vb.)
/// 
/// Sorumlulukları:
/// - CRUD operations
/// - Collection-like interface
/// - Tracking vs. No-tracking queries
/// - Query filtering ve pagination
/// 
/// Not: Sadece abstraction sağlar
/// Concrete implementation Infrastructure layer'da yapılır
/// </summary>
public interface IGenericRepository<TAggregate> where TAggregate : AggregateRoot
{
    // ==================== READ OPERATIONS ====================

    /// <summary>
    /// ID'ye göre aggregate getir
    /// </summary>
    Task<TAggregate?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Tüm aggregates'leri getir (soft deleted hariç)
    /// </summary>
    Task<IEnumerable<TAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Aggregate'in var olup olmadığını kontrol et
    /// </summary>
    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Toplam aggregate sayısını getir
    /// </summary>
    Task<int> CountAsync(
        CancellationToken cancellationToken = default);

    // ==================== WRITE OPERATIONS ====================

    /// <summary>
    /// Aggregate ekle
    /// </summary>
    Task AddAsync(
        TAggregate aggregate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Birden fazla aggregate ekle
    /// </summary>
    Task AddRangeAsync(
        IEnumerable<TAggregate> aggregates,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Aggregate güncelle
    /// </summary>
    Task UpdateAsync(
        TAggregate aggregate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Aggregate sil (soft delete)
    /// </summary>
    Task DeleteAsync(
        TAggregate aggregate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Birden fazla aggregate sil
    /// </summary>
    Task DeleteRangeAsync(
        IEnumerable<TAggregate> aggregates,
        CancellationToken cancellationToken = default);
}