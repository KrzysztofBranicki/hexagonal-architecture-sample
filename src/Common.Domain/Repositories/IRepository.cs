using System.Collections.Generic;

namespace Common.Domain.Repositories
{
    public interface IRepository<TEntity, in TId> where TEntity : IEntity<TId>
    {
        TEntity Get(TId id);
        TEntity GetIfExists(TId id);

        void Delete(TEntity entity);
        void Delete(IEnumerable<TEntity> entities);
        void Delete(TId id);
        void Delete(IEnumerable<TId> ids);

        void Add(TEntity entity);
        void Add(IEnumerable<TEntity> entities);

        void Update(TEntity entity);
        void Update(IEnumerable<TEntity> entities);
    }
}
