using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attemptation
{
    public class ManagedTryGetAttempt : ManagedTryAttempt, ITryGetAttempt
    {
        public object Result { get; set; }
    }
}
