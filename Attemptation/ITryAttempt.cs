using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attemptation
{
    public interface ITryAttempt
    {
        int Attempt { get; }
        bool Succeeded { get; }
        Exception Exception { get; }
        bool Handled { get; }
        bool Cancelled { get; }
    }
}
