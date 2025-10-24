namespace Core.Domain.Repositories;
public interface IRepository<TAggregate> : IGenericRepository<TAggregate>
    where TAggregate : AggregateRoot
{
}