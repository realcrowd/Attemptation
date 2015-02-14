using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attemptation
{
    interface ITryGetAttempt<TResult> : ITryGetAttempt
    {
        new TResult Result { get; set; }
    }
}
