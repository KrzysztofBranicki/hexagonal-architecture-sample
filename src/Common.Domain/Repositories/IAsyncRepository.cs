using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Domain.Repositories
{
    public interface IAsyncRepository<TEntity, in TId> where TEntity : IEntity<TId>
    {
        Task AddAsync(TEntity entity);
        Task AddAsync(IEnumerable<TEntity> entities);

        Task<TEntity> GetAsync(TId id);
        Task<TEntity> GetEntityOrDefaultAsync(TId id);

        Task UpdateAsync(TEntity entity);
        Task UpdateAsync(IEnumerable<TEntity> entities);

        Task DeleteAsync(TEntity entity);
        Task DeleteAsync(IEnumerable<TEntity> entities);
        Task DeleteAsync(TId id);
        Task DeleteAsync(IEnumerable<TId> ids);
    }
}
