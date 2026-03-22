using Core.Entities;
using Core.Entities.OrderAggregate;

namespace Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
    Task<int> Complete();
}