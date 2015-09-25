using System.Collections.Concurrent;

namespace Common.Domain.Repositories
{
    public class InMemoryRepository<TEntity, TId> : BaseRepository<TEntity, TId> where TEntity : class, IEntity<TId>
    {
        private readonly ConcurrentDictionary<TId, TEntity> _entities = new ConcurrentDictionary<TId, TEntity>();

        public override void Add(TEntity entity)
        {
            _entities.AddOrUpdate(entity.Id, entity, (id, entity1) => entity);
        }

        public override TEntity GetEntityOrDefault(TId id)
        {
            TEntity result;
            _entities.TryGetValue(id, out result);
            return result;
        }

        public override void Update(TEntity entity)
        {
            _entities.TryUpdate(entity.Id, entity, GetEntityOrDefault(entity.Id));
        }

        public override void Delete(TEntity entity)
        {
            TEntity result;
            _entities.TryRemove(entity.Id, out result);
        }
    }
}