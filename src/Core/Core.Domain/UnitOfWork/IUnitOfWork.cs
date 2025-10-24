namespace Core.Domain.UnitOfWork;
public interface IUnitOfWork : IAsyncDisposable
{
    Task<int> SaveChangesAsync(
    CancellationToken cancellationToken = default);
    Task<bool> BeginTransactionAsync(
    CancellationToken cancellationToken = default);
    Task<bool> CommitAsync(
    CancellationToken cancellationToken = default);
    Task<bool> RollbackAsync(
    CancellationToken cancellationToken = default);
    Task PublishDomainEventsAsync(
    CancellationToken cancellationToken = default);
}