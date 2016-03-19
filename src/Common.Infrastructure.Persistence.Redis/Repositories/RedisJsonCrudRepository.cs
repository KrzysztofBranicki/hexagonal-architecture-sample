using System.Threading.Tasks;
using Common.Domain;
using Common.Domain.Repositories;
using Common.Domain.Repositories.Exceptions;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Common.Infrastructure.Persistence.Redis.Repositories
{
    public class RedisJsonCrudRepository<TEntity, TId> : ICrudRepository<TEntity, TId>, IAsyncCrudRepository<TEntity, TId> where TEntity : class, IEntity<TId>
    {
        private static readonly string EntityName = typeof(TEntity).FullName;

        protected IDatabase Db;

        public RedisJsonCrudRepository(ConnectionMultiplexer connectionMultiplexer)
        {
            Db = connectionMultiplexer.GetDatabase();
        }

        public void Add(TEntity entity)
        {
            var added = Db.StringSet(CreateKeyFromId(entity.Id), SerializeEntity(entity));
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
            var redisValue = Db.StringGet(CreateKeyFromId(id));
            if (!redisValue.HasValue)
                return default(TEntity);

            return DeserializeEntity((string)redisValue);
        }

        public void Update(TEntity entity)
        {
            var updated = Db.StringSet(CreateKeyFromId(entity.Id), SerializeEntity(entity));
            if (!updated)
                throw new EntityUpdateFailedException(entity.Id);
        }

        public void Delete(TEntity entity)
        {
            Delete(entity.Id);
        }

        public void Delete(TId id)
        {
            var deleted = Db.KeyDelete(CreateKeyFromId(id));
            if (!deleted)
                throw new EntityDeleteFailedException(id);
        }


        public async Task AddAsync(TEntity entity)
        {
            var added = await Db.StringSetAsync(CreateKeyFromId(entity.Id), SerializeEntity(entity));
            if (!added)
                throw new EntityAddFailedException(entity.Id);
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
            var redisValue = await Db.StringGetAsync(CreateKeyFromId(id));
            if (!redisValue.HasValue)
                return default(TEntity);

            return DeserializeEntity(redisValue);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            var updated = await Db.StringSetAsync(CreateKeyFromId(entity.Id), SerializeEntity(entity));
            if (!updated)
                throw new EntityUpdateFailedException(entity.Id);
        }

        public Task DeleteAsync(TEntity entity)
        {
            return DeleteAsync(entity.Id);
        }

        public async Task DeleteAsync(TId id)
        {
            var deleted = await Db.KeyDeleteAsync(CreateKeyFromId(id));
            if (!deleted)
                throw new EntityDeleteFailedException(id);
        }

        protected virtual string CreateKeyFromId(TId id)
        {
            return $"{EntityName}:{id}";
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
