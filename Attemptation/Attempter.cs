using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attemptation
{
    public static class Attempter
    {
        private static readonly ITryManagerAsync defaultTryManager = TryManager.Create(TryManager.OriginalDefaultTotalAttempts, TryManager.OriginalDefaultAttemptRetryHandler);

        private static ITryManagerAsync DefaultTryManager
        {
            get { return defaultTryManager; }
        }

        public static Attempter<TService> Create<TService>(TService service)
        {
            return Create(service, TryManager.OriginalDefaultTotalAttempts);
        }

        public static Attempter<TService> Create<TService>(TService service, int defaultTotalAttempts)
        {
            return Create(service, defaultTotalAttempts, TryManager.OriginalDefaultAttemptRetryHandler);
        }

        public static Attempter<TService> Create<TService>(TService service, int defaultTotalAttempts, AttemptRetry defaultAttemptRetryHandler)
        {
            var manager = TryManager.CreateInternal<Attempter<TService>>(defaultTotalAttempts, defaultAttemptRetryHandler);
            manager.Service = service;

            return manager;
        }

        public static bool Try(Action attemptAction)
        {
            return DefaultTryManager.Try(attemptAction, 1);
        }

        public static bool Try(Action attemptAction, int totalAttempts)
        {
            return DefaultTryManager.Try(attemptAction, totalAttempts, null);
        }

        public static bool Try(Action attemptAction, int totalAttempts, AttemptRetry retryHandler)
        {
            return DefaultTryManager.Try(attemptAction, totalAttempts, retryHandler);
        }

        public static bool Try<TResult>(Func<TResult> getResult, out TResult result)
        {
            return DefaultTryManager.Try<TResult>(getResult, out result);
        }

        public static bool Try<TResult>(Func<TResult> getResult, out TResult result, int totalAttempts)
        {
            return DefaultTryManager.Try<TResult>(getResult, out result, totalAttempts);
        }

        public static bool Try<TResult>(Func<TResult> getResult, out TResult result, int totalAttempts, AttemptRetryGet<TResult> retryHandler)
        {
            return DefaultTryManager.Try<TResult>(getResult, out result, totalAttempts, (attempt) =>
            {
                retryHandler(attempt);
            });
        }

        public static Task<bool> TryAsync(Func<Task> attemptActionAsync)
        {
            return DefaultTryManager.TryAsync(attemptActionAsync);
        }

        public static Task<bool> TryAsync(Func<Task> attemptActionAsync, int totalAttempts)
        {
            return DefaultTryManager.TryAsync(attemptActionAsync, totalAttempts);
        }
        public static Task<bool> TryAsync(Func<Task> attemptActionAsync, int totalAttempts, AttemptRetryAsync asyncRetryHandler)
        {
            return DefaultTryManager.TryAsync(attemptActionAsync, totalAttempts, asyncRetryHandler);
        }

        public static Task<TryGetResult<TResult>> TryAsync<TResult>(Func<Task<TResult>> getResultAsync)
        {
            return DefaultTryManager.TryAsync(getResultAsync);
        }

        public static Task<TryGetResult<TResult>> TryAsync<TResult>(Func<Task<TResult>> getResultAsync, int totalAttempts)
        {
            return DefaultTryManager.TryAsync(getResultAsync, totalAttempts);
        }

        public static Task<TryGetResult<TResult>> TryAsync<TResult>(Func<Task<TResult>> getResultAsync, int totalAttempts, AttemptRetryGetAsync<TResult> asyncRetryHandler)
        {
            return DefaultTryManager.TryAsync(getResultAsync, totalAttempts, asyncRetryHandler);
        }
    }
}
