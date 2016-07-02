using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Common.Domain.Transactions;
using System.Collections.Generic;

namespace Common.Infrastructure.Persistence.Ef.Transactions
{
    /// <summary>
    /// This implementation of ITransactionExecutor guarantees only that all changes will be saved in single transaction. 
    /// All queries performed during the action won't be in the same transaction.
    /// </summary>
    public class DbContextSaveChangesTransactionExecutor : ITransactionExecutor
    {
        private readonly DbContext _dbContext;

        private readonly List<Action> _postCommitActions = new List<Action>();
        private readonly List<Func<Task>> _postCommitAsyncActions = new List<Func<Task>>();

        public DbContextSaveChangesTransactionExecutor(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void ExecuteInTransaction(Action action)
        {
            action();
            _dbContext.SaveChanges();
            ExecutePostCommitActions();
        }

        public void EnlistActionToExecuteAfterSuccessfulCommit(Action action)
        {
            _postCommitActions.Add(action);
        }

        public async Task ExecuteInTransactionAsync(Func<Task> asyncAction)
        {
            await asyncAction();
            await _dbContext.SaveChangesAsync();
            await ExecutePostCommitActionsAsync();
        }

        public Task EnlistActionToExecuteAfterSuccessfulCommitAsync(Func<Task> asyncAction)
        {
            _postCommitAsyncActions.Add(asyncAction);
            return Task.CompletedTask;
        }

        private void ExecutePostCommitActions()
        {
            foreach (var action in _postCommitActions)
            {
                action();
            }
        }

        private async Task ExecutePostCommitActionsAsync()
        {
            foreach (var asyncAction in _postCommitAsyncActions)
            {
                await asyncAction();
            }
        }
    }
}