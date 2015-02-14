using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attemptation;

namespace AttemptationUnitTests
{
    [TestClass]
    public class AttempterServiceActionTests
    {

        [TestMethod]
        public void TryReturnsFalseOnFailedAction()
        {
            var attempter = Attempter.Create(new MockService());

            var succeeded = attempter.Try(s => s.MockAction(throwException:true));

            Assert.IsFalse(succeeded);
        }
    }
}
