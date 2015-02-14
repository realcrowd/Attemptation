using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attemptation
{
    internal partial class AttemptProvider<TTryResult, TAttempt>
    {
        public Func<TAttempt, Task> RetryAsyncHandler { get; set; }
        public Func<TAttempt, Task> AttemptAsyncHandler { get; set; }

        public async Task AttemptAsync(TAttempt attempt)
        {
            if (null == AttemptAsyncHandler)
                return;

            await AttemptAsyncHandler(attempt);

            attempt.Succeeded = true;
        }

        public Task HandleRetryAsync(TAttempt attempt)
        {
            if (null == RetryAsyncHandler)
                return Task.FromResult(false);

            return RetryAsyncHandler(attempt);
        }
    }
}
