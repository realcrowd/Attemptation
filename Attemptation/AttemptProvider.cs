using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attemptation
{
    internal static class AttemptProvider
    {
        public static AttemptProvider<TTryResult, TAttempt> Create<TTryResult, TAttempt>(TTryResult tryResult, Func<TAttempt> createAttempt, Action<TAttempt> attemptHandler, Action<TAttempt> retryHandler, Action<TTryResult, TAttempt> fillResult = null)
            where TTryResult : TryResult
            where TAttempt : ManagedTryAttempt
        {
            var provider = new AttemptProvider<TTryResult, TAttempt>(tryResult);

            provider.AttemptCreator = createAttempt;
            provider.AttemptHandler = attemptHandler;
            provider.RetryHandler = retryHandler;
            provider.ResultFiller = fillResult;

            return provider;
        }

        public static AttemptProvider<TTryResult, TAttempt> CreateForAsync<TTryResult, TAttempt>(TTryResult tryResult, Func<TAttempt> createAttempt, Func<TAttempt, Task> attemptAsyncHandler, Func<TAttempt, Task> retryAsyncHandler, Action<TTryResult, TAttempt> fillResult = null)
            where TTryResult : TryResult
            where TAttempt : ManagedTryAttempt
        {
            var provider = new AttemptProvider<TTryResult, TAttempt>(tryResult);

            provider.AttemptCreator = createAttempt;
            provider.AttemptAsyncHandler = attemptAsyncHandler;
            provider.RetryAsyncHandler = retryAsyncHandler;
            provider.ResultFiller = fillResult;

            return provider;
        }
    }

    internal partial class AttemptProvider<TTryResult, TAttempt>
        where TTryResult : TryResult
        where TAttempt : ManagedTryAttempt
    {
        private TTryResult tryResult;
        public Func<TAttempt> AttemptCreator { get; set; }
        public Action<TAttempt> RetryHandler { get; set; }
        public Action<TAttempt> AttemptHandler { get; set; }
        public Action<TTryResult, TAttempt> ResultFiller { get; set; }


        public AttemptProvider(TTryResult tryResult)
        {
            this.tryResult = tryResult;
        }

        public TTryResult TryResult
        {
            get { return tryResult; }
        }

        public TAttempt CreateAttempt(int attemptCount = 1)
        {
            var attempt = AttemptCreator();
            attempt.Attempt = attemptCount;

            return attempt;
        }

        public void HandleRetry(TAttempt attempt)
        {
            if (null == RetryHandler)
                return;

            RetryHandler(attempt);
        }

        public void Attempt(TAttempt attempt)
        {
            if (null == AttemptHandler)
                return;

            AttemptHandler(attempt);

            attempt.Succeeded = true;
        }

        public void FillResult(TAttempt attempt)
        {
            tryResult.Succeeded = attempt.Succeeded;

            if (null == ResultFiller)
                return;

            ResultFiller(tryResult, attempt);
        }
    }
}
