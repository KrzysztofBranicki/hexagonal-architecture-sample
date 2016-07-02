using System;
using System.Threading.Tasks;

namespace Common.Domain.Transactions
{
    public interface ITransactionExecutor
    {
        void ExecuteInTransaction(Action action);
        void EnlistActionToExecuteAfterSuccessfulCommit(Action action);

        Task ExecuteInTransactionAsync(Func<Task> asyncAction);
        Task EnlistActionToExecuteAfterSuccessfulCommitAsync(Func<Task> asyncAction);
    }
}
