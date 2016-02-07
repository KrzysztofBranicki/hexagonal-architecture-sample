using System.Collections.Concurrent;
using System.Collections.Generic;
using Common.Domain.Repositories.Exceptions;

namespace Common.Domain.Repositories.InMemory
{
    public class RepositoryInMemoryCachingDecorator<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class, IEntity<TId>
    {
        private readonly IRepository<TEntity, TId> _decoratedRepository;
        private readonly ConcurrentDictionary<TId, TEntity> _entitiesCache = new ConcurrentDictionary<TId, TEntity>();

        public RepositoryInMemoryCachingDecorator(IRepository<TEntity, TId> decoratedRepository)
        {
            _decoratedRepository = decoratedRepository;
        }

        public void Add(TEntity entity)
        {
            _decoratedRepository.Add(entity);
            AddEntityToCache(entity);
        }

        public void Add(IEnumerable<TEntity> entities)
        {
            _decoratedRepository.Add(entities);

            foreach (var entity in entities)
                AddEntityToCache(entity);
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
            var entity = GetEntityFromCache(id);
            if (entity == null)
            {
                entity = _decoratedRepository.GetEntityOrDefault(id);
                if (entity != null)
                    AddEntityToCache(entity);
            }

            return entity;
        }

        public void Update(TEntity entity)
        {
            _decoratedRepository.Update(entity);
            UpdateEntityInCache(entity);
        }

        public void Update(IEnumerable<TEntity> entities)
        {
            _decoratedRepository.Update(entities);

            foreach (var entity in entities)
                UpdateEntityInCache(entity);
        }

        public void Delete(TEntity entity)
        {
            _decoratedRepository.Delete(entity);
            RemoveEntityFromCache(entity);
        }

        public void Delete(IEnumerable<TEntity> entities)
        {
            _decoratedRepository.Delete(entities);

            foreach (var entity in entities)
                RemoveEntityFromCache(entity);
        }

        public void Delete(TId id)
        {
            _decoratedRepository.Delete(id);
            RemoveEntityFromCacheById(id);
        }

        public void Delete(IEnumerable<TId> ids)
        {
            _decoratedRepository.Delete(ids);

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
