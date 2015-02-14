using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attemptation
{
    public interface ITryManager<TService> : ITryManager
    {
        TService Service { get; set; }
        bool Try(Action<TService> attemptServiceAction);
        bool Try(Action<TService> attemptServiceAction, int totalAttempts);
        bool Try(Action<TService> attemptServiceAction, int totalAttempts, AttemptRetry retryAttemptHandler);
        bool Try<TResult>(Func<TService, TResult> attemptServiceGet, out TResult result);
        bool Try<TResult>(Func<TService, TResult> attemptServiceGet, out TResult result, int totalAttempts);
        bool Try<TResult>(Func<TService, TResult> attemptServiceGet, out TResult result, int totalAttempts, AttemptRetryGet<TResult> retryAttemptHandler);
    }
}
