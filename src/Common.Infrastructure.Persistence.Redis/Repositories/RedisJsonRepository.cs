using System;
using System.Collections.Generic;
using Common.Domain;
using Common.Domain.Repositories;
using Common.Domain.Repositories.Exceptions;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Common.Infrastructure.Persistence.Redis.Repositories
{
    public class RedisJsonRepository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class, IEntity<TId>
    {
        protected readonly ConnectionMultiplexer ConnectionMultiplexer;
        protected IDatabase Db;

        private static readonly string EntityName = typeof (TEntity).FullName;

        public RedisJsonRepository(ConnectionMultiplexer connectionMultiplexer)
        {
            ConnectionMultiplexer = connectionMultiplexer;
            Db = ConnectionMultiplexer.GetDatabase();
        }

        public void Add(TEntity entity)
        {
            var added = Db.StringSet(CreateKeyFromId(entity.Id), SerializeEntity(entity));
            if(!added)
                throw new EntityAddFailedException(entity.Id);
        }
        
        public void Add(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                Add(entity);
        }

        public TEntity Get(TId id)
        {
            var entity = GetEntityOrDefault(id);
            if(entity == null)
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
            if(!updated)
                throw new EntityUpdateFailedException(entity.Id);
        }

        public void Update(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                Update(entity);
        }

        public void Delete(TEntity entity)
        {
            Delete(entity.Id);
        }

        public void Delete(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                Delete(entity);
        }

        public void Delete(TId id)
        {
            var deleted = Db.KeyDelete(CreateKeyFromId(id));
            if(!deleted)
                throw new EntityDeleteFailedException(id);
        }

        public void Delete(IEnumerable<TId> ids)
        {
            foreach (var id in ids)
                Delete(id);
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
