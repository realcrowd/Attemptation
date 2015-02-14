using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attemptation
{
    public delegate Task AttemptRetryAsync(ManagedTryAttempt attempt);

    public delegate Task AttemptRetryGetAsync<TResult>(ManagedTryGetAttempt<TResult> attempt);

    public partial class TryManager : ITryManagerAsync
    {
        public AttemptRetryAsync DefaultAsyncAttemptRetryHandler { get; protected set; }

        public Task<bool> TryAsync(Func<Task> attemptActionAsync)
        {
            return TryAsync(attemptActionAsync, DefaultTotalAttempts);
        }

        public Task<bool> TryAsync(Func<Task> attemptActionAsync, int totalAttempts)
        {
            return TryAsync(attemptActionAsync, totalAttempts, GetDefaultAsyncAttemptRetryHandler());
        }

        public async Task<bool> TryAsync(Func<Task> attemptActionAsync, int totalAttempts, AttemptRetryAsync asyncRetryHandler)
        {
            var tryResult = new ManagedTryResult();

            tryResult = await TryInternalAsync(
                AttemptProvider.CreateForAsync(tryResult, () => new ManagedTryGetAttempt(),
                    (attempt) => attemptActionAsync(),
                    (attempt) => (asyncRetryHandler == null) ? Task.FromResult(false) : asyncRetryHandler(attempt)),
                    totalAttempts);

            return tryResult.Succeeded;
        }

        public Task<TryGetResult<TResult>> TryAsync<TResult>(Func<Task<TResult>> getResultAsync)
        {
            return TryAsync(getResultAsync, DefaultTotalAttempts);
        }

        internal AttemptRetryAsync GetDefaultAsyncAttemptRetryHandler()
        {
            return (null != DefaultAsyncAttemptRetryHandler) ? DefaultAsyncAttemptRetryHandler : (attempt) =>
            {

                if (null != DefaultAttemptRetryHandler)
                    DefaultAttemptRetryHandler(attempt);
                return Task.FromResult(false);
            };
        }

        public Task<TryGetResult<TResult>> TryAsync<TResult>(Func<Task<TResult>> getResultAsync, int totalAttempts)
        {
            return TryAsync(getResultAsync, totalAttempts, (attempt) => {
                var defaultHandler = GetDefaultAsyncAttemptRetryHandler();
                if (null != defaultHandler)
                    return defaultHandler(attempt);

                return Task.FromResult(false);
            });
        }

        public async Task<TryGetResult<TResult>> TryAsync<TResult>(Func<Task<TResult>> getResultAsync, int totalAttempts, AttemptRetryGetAsync<TResult> asyncRetryHandler)
        {
            var tryResult = new ManagedTryGetResult<TResult>();

            tryResult = await TryInternalAsync(
                AttemptProvider.CreateForAsync(tryResult, () => new ManagedTryGetAttempt<TResult>(),
                    async (attempt) => attempt.Result = await getResultAsync(),
                    (attempt) => (asyncRetryHandler == null) ? Task.FromResult(false) : asyncRetryHandler(attempt),
                    (tr, a) =>
                    {
                        tr.Result = a.Result;
                    }), totalAttempts);

            return new TryGetResult<TResult>
            {
                Result = tryResult.Result,
                Succeeded = tryResult.Succeeded
            };
        }

        private async Task<TTryResult> TryInternalAsync<TTryResult, TAttempt>(AttemptProvider<TTryResult, TAttempt> attemptProvider, int totalAttempts)
            where TAttempt : ManagedTryAttempt
            where TTryResult : TryResult
        {
            Func<TAttempt, Task> attempterAsync = null;

            attempterAsync = async (attempt) =>
            {
                var attemptingAgain = false;

                try
                {
                    await attemptProvider.AttemptAsync(attempt);
                }
                catch (Exception ex)
                {
                    attempt.Exception = ex;
                    attemptingAgain = (totalAttempts > attempt.Attempt);
                }

                if (attemptingAgain)
                {

                    attempt = attemptProvider.CreateAttempt(attempt.Attempt + 1);
                    await attemptProvider.HandleRetryAsync(attempt);

                    if (!attempt.Handled && !attempt.Cancelled)
                        await attempterAsync(attempt);
                }

                attemptProvider.FillResult(attempt);
            };

            await attempterAsync(attemptProvider.CreateAttempt(1));

            return attemptProvider.TryResult;
        }
    }
}
