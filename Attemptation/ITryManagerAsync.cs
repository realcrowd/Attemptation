using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attemptation
{
    public interface ITryManagerAsync : ITryManager
    {
        Task<bool> TryAsync(Func<Task> attemptActionAsync);
        Task<bool> TryAsync(Func<Task> attemptActionAsync, int totalAttempts);
        Task<bool> TryAsync(Func<Task> attemptActionAsync, int totalAttempts, AttemptRetryAsync asyncRetryHandler);
        Task<TryGetResult<TResult>> TryAsync<TResult>(Func<Task<TResult>> getResultAsync);
        Task<TryGetResult<TResult>> TryAsync<TResult>(Func<Task<TResult>> getResultAsync, int totalAttempts);
        Task<TryGetResult<TResult>> TryAsync<TResult>(Func<Task<TResult>> getResultAsync, int totalAttempts, AttemptRetryGetAsync<TResult> asyncRetryHandler);
    }
}
