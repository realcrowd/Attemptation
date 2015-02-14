using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attemptation
{
    public delegate void AttemptRetry(ManagedTryAttempt attempt);

    public delegate void AttemptRetryGet<TResult>(ManagedTryGetAttempt<TResult> attempt);

    public partial class TryManager : ITryManager
    {
        internal const int OriginalDefaultTotalAttempts = 1;
        internal static readonly AttemptRetry OriginalDefaultAttemptRetryHandler = null;

        public int DefaultTotalAttempts { get; protected set; }
        public AttemptRetry DefaultAttemptRetryHandler { get; protected set; }

        public static TryManager Create()
        {
            return Create(OriginalDefaultTotalAttempts);
        }

        public static TryManager Create(int defaultTotalAttempts)
        {
            return Create(defaultTotalAttempts, OriginalDefaultAttemptRetryHandler);
        }

        public static TryManager Create(int defaultTotalAttempts, AttemptRetry defaultAttemptRetryHandler)
        {
            return CreateInternal<TryManager>(defaultTotalAttempts, defaultAttemptRetryHandler);
        }

        internal static TManager CreateInternal<TManager>(int defaultTotalAttempts, AttemptRetry defaultAttemptRetryHandler)
            where TManager : TryManager, new()
        {
            return new TManager
            {
                DefaultTotalAttempts = defaultTotalAttempts,
                DefaultAttemptRetryHandler = defaultAttemptRetryHandler
            };
        }

        public bool Try(Action attemptAction)
        {
            return Try(attemptAction, DefaultTotalAttempts);
        }

        public bool Try(Action attemptAction, int totalAttempts)
        {
            return Try(attemptAction, totalAttempts, DefaultAttemptRetryHandler);
        }

        public bool Try(Action attemptAction, int totalAttempts, AttemptRetry retryAttemptHandler)
        {
            var tryResult = TryInternal(
                AttemptProvider.Create(new ManagedTryResult(), 
                    () => new ManagedTryAttempt(),
                    (attempt) => attemptAction(),
                    (attempt) =>
                    {
                        if (null != retryAttemptHandler)
                            retryAttemptHandler(attempt);
                    }), totalAttempts);

            return tryResult.Succeeded;
        }

        public bool Try<TResult>(Func<TResult> getResult, out TResult result)
        {
            return Try(getResult, out result, DefaultTotalAttempts);
        }

        public bool Try<TResult>(Func<TResult> getResult, out TResult result, int totalAttempts)
        {
            return Try(getResult, out result, totalAttempts, (attempt) =>
            {
                if (null != DefaultAttemptRetryHandler)
                    DefaultAttemptRetryHandler(attempt);
            });
        }

        public bool Try<TResult>(Func<TResult> getResult, out TResult result, int totalAttempts, AttemptRetryGet<TResult> retryAttemptHandler)
        {
            var tryResult = new ManagedTryGetResult<TResult>();

            tryResult = TryInternal(
                AttemptProvider.Create(tryResult, () => new ManagedTryGetAttempt<TResult>(), 
                    (attempt) => attempt.Result = getResult(),
                    (attempt) =>
                    {
                        if (null != retryAttemptHandler)
                            retryAttemptHandler(attempt);
                    },
                    (tr, a) =>
                    {
                        tr.Result = a.Result;
                    }), totalAttempts);

            result = tryResult.Result;
            return tryResult.Succeeded;
        }

        private TTryResult TryInternal<TTryResult, TAttempt>(AttemptProvider<TTryResult, TAttempt> attemptProvider, int totalAttempts)
            where TAttempt : ManagedTryAttempt
            where TTryResult : TryResult
        {
            Action<TAttempt> attempter = null;

            attempter = (attempt) =>
            {
                try
                {
                    attemptProvider.Attempt(attempt);
                }
                catch (Exception ex)
                {
                    attempt.Exception = ex;
                    if (totalAttempts > attempt.Attempt)
                    {
                        attempt = attemptProvider.CreateAttempt(attempt.Attempt + 1);
                        attemptProvider.HandleRetry(attempt);

                        if (!attempt.Handled && !attempt.Cancelled)
                            attempter(attempt);
                    }
                }

                attemptProvider.FillResult(attempt);
            };

            attempter(attemptProvider.CreateAttempt(1));

            return attemptProvider.TryResult;
        }
    }
}
