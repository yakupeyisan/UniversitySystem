namespace Core.Domain.Repositories;
public interface IRepository<TEntity> : IGenericRepository<TEntity>
    where TEntity : Entity
{
}