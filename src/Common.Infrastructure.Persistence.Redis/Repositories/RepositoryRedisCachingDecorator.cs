using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Domain;
using Common.Domain.Repositories;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Common.Infrastructure.Persistence.Redis.Repositories
{
    public class RepositoryRedisCachingDecorator<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class, IEntity<TId>
    {
        private static readonly string EntityName = typeof(TEntity).FullName;

        protected IDatabase Db;

        public RepositoryRedisCachingDecorator(ConnectionMultiplexer connectionMultiplexer)
        {
            Db = connectionMultiplexer.GetDatabase();
        }

        public void Add(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Add(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public TEntity Get(TId id)
        {
            throw new NotImplementedException();
        }

        public TEntity GetEntityOrDefault(TId id)
        {
            throw new NotImplementedException();
        }

        public void Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Update(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public void Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public void Delete(TId id)
        {
            throw new NotImplementedException();
        }

        public void Delete(IEnumerable<TId> ids)
        {
            throw new NotImplementedException();
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
