using System.Threading.Tasks;

namespace Common.Domain.Repositories
{
    public interface IAsyncCrudRepository<TEntity, in TId> where TEntity : IEntity<TId>
    {
        Task AddAsync(TEntity entity);

        Task<TEntity> GetAsync(TId id);
        Task<TEntity> GetEntityOrDefaultAsync(TId id);

        Task UpdateAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);
        Task DeleteAsync(TId id);
    }
}
