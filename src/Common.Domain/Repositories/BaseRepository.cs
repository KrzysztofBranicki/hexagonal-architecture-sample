using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Domain.Repositories.Exceptions;

namespace Common.Domain.Repositories
{
    public abstract class BaseRepository<TEntity, TId> : IRepository<TEntity, TId>, IAsyncRepository<TEntity, TId> where TEntity : class, IEntity<TId>
    {
        public abstract void Add(TEntity entity);
        public abstract TEntity GetEntityOrDefault(TId id);
        public abstract void Update(TEntity entity);
        public abstract void Delete(TEntity entity);

        public abstract Task AddAsync(TEntity entity);
        public abstract Task<TEntity> GetEntityOrDefaultAsync(TId id);
        public abstract Task UpdateAsync(TEntity entity);
        public abstract Task DeleteAsync(TEntity entity);


        public virtual void Add(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                Add(entity);
        }

        public virtual TEntity Get(TId id)
        {
            var entity = GetEntityOrDefault(id);
            if (entity == null)
                throw new EntityNotFountException(id);

            return entity;
        }

        public virtual void Update(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                Update(entity);
        }

        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities.ToList())
                Delete(entity);
        }

        public virtual void Delete(TId id)
        {
            var entity = Get(id);
            Delete(entity);
        }

        public virtual void Delete(IEnumerable<TId> ids)
        {
            foreach (var id in ids)
                Delete(id);
        }

        
        public virtual async Task AddAsync(IEnumerable<TEntity> entities)
        {
            var tasks = entities.Select(AddAsync);
            await Task.WhenAll(tasks);
        }

        public virtual async Task<TEntity> GetAsync(TId id)
        {
            var entity = await GetEntityOrDefaultAsync(id);
            if (entity == null)
                throw new EntityNotFountException(id);

            return entity;
        }

        public virtual async Task UpdateAsync(IEnumerable<TEntity> entities)
        {
            var tasks = entities.Select(UpdateAsync);
            await Task.WhenAll(tasks);
        }
        
        public virtual async Task DeleteAsync(IEnumerable<TEntity> entities)
        {
            var tasks = entities.Select(DeleteAsync);
            await Task.WhenAll(tasks);
        }

        public virtual async Task DeleteAsync(TId id)
        {
            var entity = await GetAsync(id);
            await DeleteAsync(entity);
        }

        public virtual async Task DeleteAsync(IEnumerable<TId> ids)
        {
            var tasks = ids.Select(DeleteAsync);
            await Task.WhenAll(tasks);
        }
    }
}
