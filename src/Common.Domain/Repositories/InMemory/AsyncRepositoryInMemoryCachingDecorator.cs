using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Domain.Repositories.Exceptions;

namespace Common.Domain.Repositories.InMemory
{
    public class AsyncRepositoryInMemoryCachingDecorator<TEntity, TId> : IAsyncRepository<TEntity, TId> where TEntity : class, IEntity<TId>
    {
        private readonly IAsyncRepository<TEntity, TId> _decoratedRepository;
        private readonly ConcurrentDictionary<TId, TEntity> _entitiesCache = new ConcurrentDictionary<TId, TEntity>();

        public AsyncRepositoryInMemoryCachingDecorator(IAsyncRepository<TEntity, TId> decoratedRepository)
        {
            _decoratedRepository = decoratedRepository;
        }

        public async Task AddAsync(TEntity entity)
        {
            await _decoratedRepository.AddAsync(entity);
            AddEntityToCache(entity);
        }

        public async Task AddAsync(IEnumerable<TEntity> entities)
        {
            await _decoratedRepository.AddAsync(entities);
            foreach (var entity in entities)
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
            UpdateEntityInCache(entity);
        }

        public async Task UpdateAsync(IEnumerable<TEntity> entities)
        {
            await _decoratedRepository.UpdateAsync(entities);
            foreach (var entity in entities)
                UpdateEntityInCache(entity);
        }

        public async Task DeleteAsync(TEntity entity)
        {
            await _decoratedRepository.DeleteAsync(entity);
            RemoveEntityFromCache(entity);
        }

        public async Task DeleteAsync(IEnumerable<TEntity> entities)
        {
            await _decoratedRepository.DeleteAsync(entities);
            foreach (var entity in entities)
                RemoveEntityFromCache(entity);
        }

        public async Task DeleteAsync(TId id)
        {
            await _decoratedRepository.DeleteAsync(id);
            RemoveEntityFromCacheById(id);
        }

        public async Task DeleteAsync(IEnumerable<TId> ids)
        {
            await _decoratedRepository.DeleteAsync(ids);
            foreach (var id in ids)
                RemoveEntityFromCacheById(id);
        }

        protected virtual void AddEntityToCache(TEntity entity)
        {
            _entitiesCache.AddOrUpdate(entity.Id, entity, (id, entity1) => entity);
        }

        protected virtual TEntity GetEntityFromCache(TId id)
        {
            TEntity result;
            _entitiesCache.TryGetValue(id, out result);
            return result;
        }

        protected virtual void UpdateEntityInCache(TEntity entity)
        {
            _entitiesCache.TryUpdate(entity.Id, entity, GetEntityFromCache(entity.Id));
        }

        protected virtual void RemoveEntityFromCache(TEntity entity)
        {
            TEntity result;
            _entitiesCache.TryRemove(entity.Id, out result);
        }

        protected virtual void RemoveEntityFromCacheById(TId id)
        {
            TEntity result;
            _entitiesCache.TryRemove(id, out result);
        }
    }
}
