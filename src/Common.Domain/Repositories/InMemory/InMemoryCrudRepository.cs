using System.Collections.Concurrent;
using System.Threading.Tasks;
using Common.Domain.Repositories.Exceptions;

namespace Common.Domain.Repositories.InMemory
{
    public class InMemoryCrudRepository<TEntity, TId> : ICrudRepository<TEntity, TId>, IAsyncCrudRepository<TEntity, TId> where TEntity : class, IEntity<TId>
    {
        private readonly ConcurrentDictionary<TId, TEntity> _entities = new ConcurrentDictionary<TId, TEntity>();

        public void Add(TEntity entity)
        {
            var added = _entities.TryAdd(entity.Id, entity);
            if (!added)
                throw new EntityAddFailedException(entity.Id);
        }

        public TEntity Get(TId id)
        {
            var entity = GetEntityOrDefault(id);
            if (entity == null)
                throw new EntityNotFountException(id);

            return entity;
        }

        public TEntity GetEntityOrDefault(TId id)
        {
            TEntity result;
            _entities.TryGetValue(id, out result);
            return result;
        }

        public void Update(TEntity entity)
        {
            _entities.TryUpdate(entity.Id, entity, GetEntityOrDefault(entity.Id));
        }

        public void Delete(TEntity entity)
        {
            Delete(entity.Id);
        }

        public void Delete(TId id)
        {
            TEntity result;
            var deleted = _entities.TryRemove(id, out result);
            if (!deleted)
                throw new EntityDeleteFailedException(id);
        }

        public Task AddAsync(TEntity entity)
        {
            Add(entity);
            return Task.CompletedTask;
        }

        public async Task<TEntity> GetAsync(TId id)
        {
            var entity = await GetEntityOrDefaultAsync(id);
            if (entity == null)
                throw new EntityNotFountException(id);

            return entity;
        }

        public Task<TEntity> GetEntityOrDefaultAsync(TId id)
        {
            var entity = GetEntityOrDefault(id);
            return Task.FromResult(entity);
        }

        public Task UpdateAsync(TEntity entity)
        {
            Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(TEntity entity)
        {
            Delete(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(TId id)
        {
            Delete(id);
            return Task.CompletedTask;
        }
    }
}