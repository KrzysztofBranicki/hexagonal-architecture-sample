using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common.TransientExceptionHandling
{
    public class Retryer
    {
        public async Task<TResult> ExecuteWithRetryAsync<TException, TResult>(Func<Task<TResult>> action, int retryCount, TimeSpan? retryDelay = null) where TException : Exception
        {
            var result = default(TResult);
            await ExecuteWithRetryAsync<TException>(async () =>
            {
                result = await action();
            }, retryCount, retryDelay);

            return result;
        }

        public async Task ExecuteWithRetryAsync<TException>(Func<Task> action, int retryCount, TimeSpan? retryDelay = null) where TException : Exception
        {
            for (var currentRetry = 1; currentRetry <= retryCount; currentRetry++)
            {
                try
                {
                    await action();
                    return;
                }
                catch (TException) when (currentRetry < retryCount)
                {
                    if (retryDelay != null)
                        await Task.Delay(retryDelay.Value);
                }
            }
        }

        public TResult ExecuteWithRetry<TException, TResult>(Func<TResult> action, int retryCount, TimeSpan? retryDelay = null) where TException : Exception
        {
            var result = default(TResult);
            ExecuteWithRetry<TException>(() =>
            {
                result = action();
            }, retryCount, retryDelay);

            return result;
        }

        public void ExecuteWithRetry<TException>(Action action, int retryCount, TimeSpan? retryDelay = null) where TException : Exception
        {
            for (var currentRetry = 1; currentRetry <= retryCount; currentRetry++)
            {
                try
                {
                    action();
                    return;
                }
                catch (TException) when (currentRetry < retryCount)
                {
                    if (retryDelay != null)
                        Thread.Sleep(retryDelay.Value);
                }
            }
        }
    }
}
