using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attemptation
{
    public class ManagedTryAttempt : ITryAttempt
    {
        public int Attempt { get; internal set; }

        public bool Succeeded { get; set; }

        public Exception Exception { get; internal set; }

        public bool Handled { get; set; }

        public bool Cancelled { get; set; }
    }
}
