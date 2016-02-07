using System.Collections.Concurrent;
using System.Threading.Tasks;
using Common.Domain.Repositories.Exceptions;

namespace Common.Domain.Repositories.InMemory
{
    public class InMemoryRepository<TEntity, TId> : BaseRepository<TEntity, TId> where TEntity : class, IEntity<TId>
    {
        private readonly ConcurrentDictionary<TId, TEntity> _entities = new ConcurrentDictionary<TId, TEntity>();

        public override void Add(TEntity entity)
        {
            var added = _entities.TryAdd(entity.Id, entity);
            if(!added)
                throw new EntityAddFailedException(entity.Id);
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
            var deleted = _entities.TryRemove(entity.Id, out result);
            if(!deleted)
                throw new EntityDeleteFailedException(entity.Id);
        }

        public override Task AddAsync(TEntity entity)
        {
            Add(entity);
            return Task.CompletedTask;
        }

        public override Task<TEntity> GetEntityOrDefaultAsync(TId id)
        {
            var entity = GetEntityOrDefault(id);
            return Task.FromResult(entity);
        }

        public override Task UpdateAsync(TEntity entity)
        {
            Update(entity);
            return Task.CompletedTask;
        }

        public override Task DeleteAsync(TEntity entity)
        {
            Delete(entity);
            return Task.CompletedTask;
        }
    }
}