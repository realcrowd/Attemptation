using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attemptation
{
    public class ManagedTryGetAttempt<TResult> : ManagedTryGetAttempt, ITryGetAttempt<TResult>
    {
        public new TResult Result
        {
            get { return (base.Result == null) ? default(TResult) : (TResult)base.Result; }
            set { base.Result = value; }
        }
    }
}
