namespace Core.Domain.UnitOfWork;

/// <summary>
/// IUnitOfWork - Transaction management
/// 
/// UnitOfWork Pattern:
/// - Tüm repository'lerin merkezi erişim noktası
/// - Transaction management (BeginTransaction, Commit, Rollback)
/// - Database changes'ı koordine eder
/// - SaveChanges'i bir kere çağırma garantisi
/// 
/// Avantajlar:
/// - Consistency: Tüm repository'ler aynı context'i kullanır
/// - Transaction management: Tüm changes atomic'tir
/// - Simplified API: Single entry point
/// 
/// Not: Generic repositories'leri property olarak expose eder
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    // ==================== REPOSITORIES ====================
    // Module repositories burada property olarak expose edilir
    // Örnek:
    // IPersonRepository Persons { get; }
    // IStudentRepository Students { get; }
    // IPaymentRepository Payments { get; }

    // ==================== TRANSACTION MANAGEMENT ====================

    /// <summary>
    /// Tüm changes'leri database'e kaydet
    /// </summary>
    /// <returns>Etkilenen row sayısı</returns>
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Transaction başlat (explicit transaction)
    /// </summary>
    Task<bool> BeginTransactionAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Transaction commit
    /// </summary>
    Task<bool> CommitAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Transaction rollback
    /// </summary>
    Task<bool> RollbackAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Domain events'leri publish et (MediatR üzerinden)
    /// </summary>
    Task PublishDomainEventsAsync(
        CancellationToken cancellationToken = default);
}