using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Common.Domain.Transactions;
using System.Collections.Generic;

namespace Common.Infrastructure.Persistence.Ef.Transactions
{
    /// <summary>
    /// This implementation of ITransactionExecutor guarantees that all operations on DbContext will be done in the same transaction (including querying and saving data).
    /// In case of nesting there will be only one topmost transaction.
    /// </summary>
    public class DbContextTransactionExecutor : ITransactionExecutor
    {
        private readonly DbContext _dbContext;

        private readonly List<Action> _postCommitActions = new List<Action>();
        private readonly List<Func<Task>> _postCommitAsyncActions = new List<Func<Task>>();

        public DbContextTransactionExecutor(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void ExecuteInTransaction(Action action)
        {
            if (WeAreAlreadyRunningInsideTransaction())
            {
                action();
                _dbContext.SaveChanges();
            }
            else
            {
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        action();
                        _dbContext.SaveChanges();
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                    ExecutePostCommitActions();
                }
            }
        }

        public void EnlistActionToExecuteAfterSuccessfulCommit(Action action)
        {
            if (WeAreAlreadyRunningInsideTransaction())
            {
                _postCommitActions.Add(action);
            }
            else
            {
                action();
            }
        }

        public async Task ExecuteInTransactionAsync(Func<Task> asyncAction)
        {
            if (WeAreAlreadyRunningInsideTransaction())
            {
                await asyncAction();
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        await asyncAction();
                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                    await ExecutePostCommitActionsAsync();
                }
            }
        }

        public async Task EnlistActionToExecuteAfterSuccessfulCommitAsync(Func<Task> asyncAction)
        {
            if (WeAreAlreadyRunningInsideTransaction())
            {
                _postCommitAsyncActions.Add(asyncAction);
            }
            else
            {
                await asyncAction();
            }
        }

        private bool WeAreAlreadyRunningInsideTransaction()
        {
            return _dbContext.Database.CurrentTransaction != null;
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
