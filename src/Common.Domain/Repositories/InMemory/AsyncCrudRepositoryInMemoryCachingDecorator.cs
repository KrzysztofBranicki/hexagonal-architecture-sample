using System.Collections.Concurrent;
using System.Threading.Tasks;
using Common.Domain.Repositories.Exceptions;

namespace Common.Domain.Repositories.InMemory
{
    public class AsyncCrudRepositoryInMemoryCachingDecorator<TEntity, TId> : IAsyncCrudRepository<TEntity, TId> where TEntity : class, IEntity<TId>
    {
        private readonly IAsyncCrudRepository<TEntity, TId> _decoratedRepository;
        private readonly ConcurrentDictionary<TId, TEntity> _entitiesCache = new ConcurrentDictionary<TId, TEntity>();

        public AsyncCrudRepositoryInMemoryCachingDecorator(IAsyncCrudRepository<TEntity, TId> decoratedRepository)
        {
            _decoratedRepository = decoratedRepository;
        }

        public async Task AddAsync(TEntity entity)
        {
            await _decoratedRepository.AddAsync(entity);
            AddEntityToCache(entity);
        }

        public async Task<TEntity> GetAsync(TId id)
        {
            var entity = await GetEntityOrDefaultAsync(id);
            if (entity == null)
                throw new EntityNotFountException(id);

            return entity;
        }

        public async Task<TEntity> GetEntityOrDefaultAsync(TId id)
        {
            var entity = GetEntityFromCache(id);
            if (entity == null)
            {
                entity = await _decoratedRepository.GetEntityOrDefaultAsync(id);
                if (entity != null)
                    AddEntityToCache(entity);
            }

            return entity;
        }

        public async Task UpdateAsync(TEntity entity)
        {
            await _decoratedRepository.UpdateAsync(entity);
            _entitiesCache.TryUpdate(entity.Id, entity, GetEntityFromCache(entity.Id));
        }

        public async Task DeleteAsync(TEntity entity)
        {
            await _decoratedRepository.DeleteAsync(entity);
            RemoveEntityFromCacheById(entity.Id);
        }

        public async Task DeleteAsync(TId id)
        {
            await _decoratedRepository.DeleteAsync(id);
            RemoveEntityFromCacheById(id);
        }

        private void AddEntityToCache(TEntity entity)
        {
            _entitiesCache.AddOrUpdate(entity.Id, entity, (id, entity1) => entity);
        }

        private TEntity GetEntityFromCache(TId id)
        {
            TEntity result;
            _entitiesCache.TryGetValue(id, out result);
            return result;
        }

        private void RemoveEntityFromCacheById(TId id)
        {
            TEntity result;
            _entitiesCache.TryRemove(id, out result);
        }
    }
}
