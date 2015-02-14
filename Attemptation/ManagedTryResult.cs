using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attemptation
{
    public class ManagedTryResult : TryResult, ITryAttempt
    {
        public Exception Exception { get; set; }
        public bool Handled { get; set; }
        public bool Cancelled { get; set; }

        public int TotalAttempts { get; set; }
        public ManagedTryAttempt[] Attempts { get; set; }

        int ITryAttempt.Attempt { get { return 0; } }
    }
}
