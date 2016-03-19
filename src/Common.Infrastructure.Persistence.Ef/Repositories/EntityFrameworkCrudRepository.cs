using System.Data.Entity;
using System.Threading.Tasks;
using Common.Domain;
using Common.Domain.Repositories;
using Common.Domain.Repositories.Exceptions;

namespace Common.Infrastructure.Persistence.Ef.Repositories
{
    public class EntityFrameworkCrudRepository<TEntity, TId> : ICrudRepository<TEntity, TId>, IAsyncCrudRepository<TEntity, TId> where TEntity : class, IEntity<TId>
    {
        protected readonly DbContext DbContext;
        protected readonly DbSet<TEntity> DbSet;

        public EntityFrameworkCrudRepository(DbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<TEntity>();
        }

        public void Add(TEntity entity)
        {
            DbSet.Add(entity);
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
            return DbSet.Find(id);
        }

        public void Update(TEntity entity)
        {
            //In case entity is not tracked by EF (it doesnt come from DbContext but instead from some sort of cache like Redis) we need to explicitly tell EF about it
            DbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(TEntity entity)
        {
            DbSet.Attach(entity);//in case it's not already attached
            DbSet.Remove(entity);
        }

        public void Delete(TId id)
        {
            DbSet.Remove(Get(id));
        }

        public Task AddAsync(TEntity entity)
        {
            Add(entity);
            return Task.CompletedTask;
        }

        public async Task<TEntity> GetAsync(TId id)
        {
            var entity = await GetEntityOrDefaultAsync(id);
            if (entity == null)
                throw new EntityNotFountException(id);

            return entity;
        }

        public Task<TEntity> GetEntityOrDefaultAsync(TId id)
        {
            return DbSet.FindAsync(id);
        }

        public Task UpdateAsync(TEntity entity)
        {
            Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(TEntity entity)
        {
            Delete(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(TId id)
        {
            Delete(await GetAsync(id));
        }
    }
}
