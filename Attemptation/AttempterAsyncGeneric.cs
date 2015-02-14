using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attemptation
{
    public partial class Attempter<TService>
    {
        public Task<bool> TryAsync(Func<TService, Task> attemptServiceActionAsync)
        {
            return TryAsync(attemptServiceActionAsync, DefaultTotalAttempts);
        }

        public Task<bool> TryAsync(Func<TService, Task> attemptServiceActionAsync, int totalAttempts)
        {
            return TryAsync(attemptServiceActionAsync, totalAttempts, GetDefaultAsyncAttemptRetryHandler());
        }

        public Task<bool> TryAsync(Func<TService, Task> attemptServiceActionAsync, int totalAttempts, AttemptRetryAsync asyncRetryHandler)
        {
            return TryAsync(() => attemptServiceActionAsync(Service), totalAttempts, asyncRetryHandler);
        }

        public Task<TryGetResult<TResult>> TryAsync<TResult>(Func<TService, Task<TResult>> getServiceResultAsync)
        {
            return TryAsync(getServiceResultAsync, DefaultTotalAttempts);
        }

        public Task<TryGetResult<TResult>> TryAsync<TResult>(Func<TService, Task<TResult>> getServiceResultAsync, int totalAttempts)
        {
            return TryAsync(getServiceResultAsync, totalAttempts, (attempt) =>
            {
                var defaultHandler = GetDefaultAsyncAttemptRetryHandler();
                if (null != defaultHandler)
                    return defaultHandler(attempt);

                return Task.FromResult(false);
            });
        }

        public Task<TryGetResult<TResult>> TryAsync<TResult>(Func<TService, Task<TResult>> getServiceResultAsync, int totalAttempts, AttemptRetryGetAsync<TResult> asyncRetryHandler)
        {
            return TryAsync(() => getServiceResultAsync(Service), totalAttempts, asyncRetryHandler);
        }
    }
}
