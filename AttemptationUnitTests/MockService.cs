using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttemptationUnitTests
{
    public class MockService
    {
        public void MockAction(bool throwException = false)
        {
            if (throwException)
                throw new Exception();
        }
    }
}
