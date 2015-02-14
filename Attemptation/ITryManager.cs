using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attemptation
{
    public interface ITryManager
    {
        int DefaultTotalAttempts { get; }
        AttemptRetry DefaultAttemptRetryHandler { get; }

        bool Try(Action attemptAction);
        bool Try(Action attemptAction, int totalAttempts);
        bool Try(Action attemptAction, int totalAttempts, AttemptRetry retryCallback);
        bool Try<TResult>(Func<TResult> getResult, out TResult result);
        bool Try<TResult>(Func<TResult> getResult, out TResult result, int totalAttempts);
        bool Try<TResult>(Func<TResult> getResult, out TResult result, int totalAttempts, AttemptRetryGet<TResult> retryCallback);

    }
}
