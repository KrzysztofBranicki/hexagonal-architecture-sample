using System.Collections.Generic;
using System.Linq;

namespace Common.Domain.Repositories
{
    public abstract class BaseRepository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class, IEntity<TId>
    {
        public virtual TEntity Get(TId id)
        {
            var entity = GetIfExists(id);
            if (entity == null)
                throw new EntityNotFountException(id);

            return entity;
        }

        public abstract TEntity GetIfExists(TId id);

        public abstract void Delete(TEntity entity);

        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities.ToList())
                Delete(entity);
        }

        public virtual void Delete(TId id)
        {
            var entity = Get(id);
            Delete(entity);
        }

        public virtual void Delete(IEnumerable<TId> ids)
        {
            foreach (var id in ids)
                Delete(id);
        }

        public abstract void Add(TEntity entity);

        public virtual void Add(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                Add(entity);
        }

        public abstract void Update(TEntity entity);

        public virtual void Update(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                Update(entity);
        }
    }
}
