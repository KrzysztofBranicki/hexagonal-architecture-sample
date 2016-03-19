using System.Threading.Tasks;
using Common.Domain;
using Common.Domain.Repositories;
using Common.Domain.Repositories.Exceptions;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Common.Infrastructure.Persistence.Redis.Repositories
{
    public class AsyncCrudRepositoryRedisCachingDecorator<TEntity, TId> : IAsyncCrudRepository<TEntity, TId> where TEntity : class, IEntity<TId>
    {
        private static readonly string EntityName = typeof(TEntity).FullName;

        private readonly IAsyncCrudRepository<TEntity, TId> _decoratedRepository;
        protected IDatabase Db;

        public AsyncCrudRepositoryRedisCachingDecorator(IAsyncCrudRepository<TEntity, TId> decoratedRepository, ConnectionMultiplexer connectionMultiplexer)
        {
            _decoratedRepository = decoratedRepository;
            Db = connectionMultiplexer.GetDatabase();
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
            Db.StringSet(CreateKeyFromId(entity.Id), SerializeEntity(entity));
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
