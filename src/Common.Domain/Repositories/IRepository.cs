using System.Collections.Generic;

namespace Common.Domain.Repositories
{
    public interface IRepository<TEntity, in TId> where TEntity : IEntity<TId>
    {
        void Add(TEntity entity);
        void Add(IEnumerable<TEntity> entities);

        TEntity Get(TId id);
        TEntity GetEntityOrDefault(TId id);
        
        void Update(TEntity entity);
        void Update(IEnumerable<TEntity> entities);

        void Delete(TEntity entity);
        void Delete(IEnumerable<TEntity> entities);
        void Delete(TId id);
        void Delete(IEnumerable<TId> ids);
    }
}
