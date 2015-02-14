using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attemptation
{
    interface ITryManagerAsync<TService> : ITryManagerAsync
    {
        Task<bool> TryAsync(Func<TService, Task> attemptServiceActionAsync);
        Task<bool> TryAsync(Func<TService, Task> attemptServiceActionAsync, int totalAttempts);
        Task<bool> TryAsync(Func<TService, Task> attemptServiceActionAsync, int totalAttempts, AttemptRetryAsync asyncRetryHandler);
        Task<TryGetResult<TResult>> TryAsync<TResult>(Func<TService, Task<TResult>> getServiceResultAsync);
        Task<TryGetResult<TResult>> TryAsync<TResult>(Func<TService, Task<TResult>> getServiceResultAsync, int totalAttempts);
        Task<TryGetResult<TResult>> TryAsync<TResult>(Func<TService, Task<TResult>> getServiceResultAsync, int totalAttempts, AttemptRetryGetAsync<TResult> asyncRetryHandler);
    }
}
