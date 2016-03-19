using System.Collections.Concurrent;
using Common.Domain.Repositories.Exceptions;

namespace Common.Domain.Repositories.InMemory
{
    public class CrudRepositoryInMemoryCachingDecorator<TEntity, TId> : ICrudRepository<TEntity, TId> where TEntity : class, IEntity<TId>
    {
        private readonly ICrudRepository<TEntity, TId> _decoratedRepository;
        private readonly ConcurrentDictionary<TId, TEntity> _entitiesCache = new ConcurrentDictionary<TId, TEntity>();

        public CrudRepositoryInMemoryCachingDecorator(ICrudRepository<TEntity, TId> decoratedRepository)
        {
            _decoratedRepository = decoratedRepository;
        }

        public void Add(TEntity entity)
        {
            _decoratedRepository.Add(entity);
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
            _entitiesCache.TryUpdate(entity.Id, entity, GetEntityFromCache(entity.Id));
        }

        public void Delete(TEntity entity)
        {
            _decoratedRepository.Delete(entity);
            RemoveEntityFromCacheById(entity.Id);
        }

        public void Delete(TId id)
        {
            _decoratedRepository.Delete(id);
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
