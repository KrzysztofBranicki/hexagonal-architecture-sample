using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Domain.Repositories
{
    public static class RepositoryExtensions
    {
        public static void Add<TEntity, TId>(this IRepository<TEntity, TId> repository, IEnumerable<TEntity> entities) where TEntity : IEntity<TId>
        {
            foreach (var entity in entities)
                repository.Add(entity);
        }

        public static void Update<TEntity, TId>(this IRepository<TEntity, TId> repository, IEnumerable<TEntity> entities) where TEntity : IEntity<TId>
        {
            foreach (var entity in entities)
                repository.Update(entity);
        }

        public static void Delete<TEntity, TId>(this IRepository<TEntity, TId> repository, IEnumerable<TEntity> entities) where TEntity : IEntity<TId>
        {
            foreach (var entity in entities)
                repository.Delete(entity);
        }

        public static void Delete<TEntity, TId>(this IRepository<TEntity, TId> repository, IEnumerable<TId> ids) where TEntity : IEntity<TId>
        {
            foreach (var id in ids)
                repository.Delete(id);
        }

        public static Task AddAsync<TEntity, TId>(this IAsyncRepository<TEntity, TId> repository, IEnumerable<TEntity> entities) where TEntity : IEntity<TId>
        {
            var tasks = entities.Select(repository.AddAsync);
            return Task.WhenAll(tasks);
        }

        public static Task UpdateAsync<TEntity, TId>(this IAsyncRepository<TEntity, TId> repository, IEnumerable<TEntity> entities) where TEntity : IEntity<TId>
        {
            var tasks = entities.Select(repository.UpdateAsync);
            return Task.WhenAll(tasks);
        }

        public static Task DeleteAsync<TEntity, TId>(this IAsyncRepository<TEntity, TId> repository, IEnumerable<TEntity> entities) where TEntity : IEntity<TId>
        {
            var tasks = entities.Select(repository.DeleteAsync);
            return Task.WhenAll(tasks);
        }

        public static Task DeleteAsync<TEntity, TId>(this IAsyncRepository<TEntity, TId> repository, IEnumerable<TId> ids) where TEntity : IEntity<TId>
        {
            var tasks = ids.Select(repository.DeleteAsync);
            return Task.WhenAll(tasks);
        }
    }
}
