using System;
using System.Threading.Tasks;

namespace Common.Domain.Transactions
{
    public interface ITransactionExecutor
    {
        void ExecuteInTransaction(Action action);
        Task ExecuteInTransactionAsync(Func<Task> asyncAction);
    }
}
