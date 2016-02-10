using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Common.Domain.Transactions;

namespace Common.Infrastructure.Persistence.Ef.Transactions
{
    /// <summary>
    /// This implementation of ITransactionExecutor guarantees only that all changes will be saved in single transaction. 
    /// All queries performed during the action won't be in the same transaction.
    /// </summary>
    public class DbContextSaveChangesTransactionExecutor : ITransactionExecutor
    {
        private readonly DbContext _dbContext;

        public DbContextSaveChangesTransactionExecutor(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void ExecuteInTransaction(Action action)
        {
            action();
            _dbContext.SaveChanges();
        }

        public async Task ExecuteInTransactionAsync(Func<Task> asyncAction)
        {
            await asyncAction();
            await _dbContext.SaveChangesAsync();
        }
    }
}