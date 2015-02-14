using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attemptation
{
    public class TryAttempt
    {
        public int Attempt { get; set; }
        public bool Succeeded { get; set; }
        public Exception Exception { get; set; }
        public bool Handled { get; set; }
        public bool Cancelled { get; set; }
    }
}
