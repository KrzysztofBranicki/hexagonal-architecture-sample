namespace Common.Domain.Repositories
{
    public interface ICrudRepository<TEntity, in TId> where TEntity : IEntity<TId>
    {
        void Add(TEntity entity);

        TEntity Get(TId id);
        TEntity GetEntityOrDefault(TId id);

        void Update(TEntity entity);

        void Delete(TEntity entity);
        void Delete(TId id);
    }
}
