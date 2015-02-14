using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attemptation
{
    public partial class Attempter<TService> : TryManager
    {
        public TService Service { get; set; }

        public bool Try(Action<TService> attemptServiceAction)
        {
            return Try(attemptServiceAction, DefaultTotalAttempts);
        }

        public bool Try(Action<TService> attemptServiceAction, int totalAttempts)
        {
            return Try(attemptServiceAction, totalAttempts, DefaultAttemptRetryHandler);
        }

        public bool Try(Action<TService> attemptServiceAction, int totalAttempts, AttemptRetry retryAttemptHandler)
        {
            return Try(() => attemptServiceAction(Service), totalAttempts, retryAttemptHandler);
        }

        public bool Try<TResult>(Func<TService, TResult> attemptServiceGet, out TResult result)
        {
            return Try(attemptServiceGet, out result, DefaultTotalAttempts);
        }

        public bool Try<TResult>(Func<TService, TResult> attemptServiceGet, out TResult result, int totalAttempts)
        {
            return Try(attemptServiceGet, out result, totalAttempts, (attempt) =>
            {
                if (null != DefaultAttemptRetryHandler)
                    DefaultAttemptRetryHandler(attempt);
            });
        }

        public bool Try<TResult>(Func<TService, TResult> attemptServiceGet, out TResult result, int totalAttempts, AttemptRetryGet<TResult> retryAttemptHandler)
        {
            return Try(() => attemptServiceGet(Service), out result, totalAttempts, retryAttemptHandler);
        }
    }
}
