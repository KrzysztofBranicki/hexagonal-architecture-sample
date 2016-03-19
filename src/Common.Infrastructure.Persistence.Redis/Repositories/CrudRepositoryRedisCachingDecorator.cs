using Common.Domain;
using Common.Domain.Repositories;
using Common.Domain.Repositories.Exceptions;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Common.Infrastructure.Persistence.Redis.Repositories
{
    public class CrudRepositoryRedisCachingDecorator<TEntity, TId> : ICrudRepository<TEntity, TId> where TEntity : class, IEntity<TId>
    {
        private static readonly string EntityName = typeof(TEntity).FullName;

        private readonly ICrudRepository<TEntity, TId> _decoratedRepository;
        protected IDatabase Db;

        public CrudRepositoryRedisCachingDecorator(ICrudRepository<TEntity, TId> decoratedRepository, ConnectionMultiplexer connectionMultiplexer)
        {
            _decoratedRepository = decoratedRepository;
            Db = connectionMultiplexer.GetDatabase();
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
            Db.StringSet(CreateKeyFromId(entity.Id), SerializeEntity(entity));
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
            Db.StringSet(CreateKeyFromId(entity.Id), SerializeEntity(entity));
        }

        private TEntity GetEntityFromCache(TId id)
        {
            var redisValue = Db.StringGet(CreateKeyFromId(id));
            if (!redisValue.HasValue)
                return default(TEntity);

            return DeserializeEntity(redisValue);
        }

        private void RemoveEntityFromCacheById(TId id)
        {
            Db.KeyDelete(CreateKeyFromId(id));
        }

        protected virtual string CreateKeyFromId(TId id)
        {
            return $"Cache:{EntityName}:{id}";
        }

        protected virtual string SerializeEntity(TEntity entity)
        {
            return JsonConvert.SerializeObject(entity);
        }

        protected virtual TEntity DeserializeEntity(string jsonEntity)
        {
            return JsonConvert.DeserializeObject<TEntity>(jsonEntity);
        }
    }
}
