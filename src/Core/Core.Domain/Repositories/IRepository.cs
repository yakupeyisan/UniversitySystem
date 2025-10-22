namespace Core.Domain.Repositories;

/// <summary>
/// IRepository - Module-specific repository interface marker
/// 
/// Kullanım:
/// public interface IPersonRepository : IRepository<Person>
/// {
///     Task<Person> GetByNationalIdAsync(string nationalId);
/// }
/// </summary>
public interface IRepository<TAggregate> : IGenericRepository<TAggregate>
    where TAggregate : AggregateRoot
{
}