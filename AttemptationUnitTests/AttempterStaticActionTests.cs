using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Attemptation;

namespace AttemptationUnitTests
{
    [TestClass]
    public class AttempterStaticActionTests
    {
        [TestMethod]
        public void TryReturnsFalseOnFailedAction()
        {
            var succeeded = Attempter.Try(() => {
                throw new Exception();
            });

            Assert.IsFalse(succeeded);
        }

        [TestMethod]
        public void TryReturnsTrueOnSucceededAction()
        {
            var succeeded = Attempter.Try(() =>
            {
                return;
            });

            Assert.IsTrue(succeeded);
        }

        [TestMethod]
        public void TryAttemptsOnceByDefault()
        {
            int attemptCount = 0;

            var succeeded = Attempter.Try(() =>
            {
                attemptCount++;
                throw new Exception();
            });

            Assert.AreEqual(attemptCount, 1);
            Assert.IsFalse(succeeded);
        }

        [TestMethod]
        public void TryAttemptsBasedOnParameter()
        {
            int attemptCount = 0;

            var succeeded = Attempter.Try(() =>
            {
                attemptCount++;
                throw new Exception();
            }, 10);

            Assert.AreEqual(attemptCount, 10);
            Assert.IsFalse(succeeded);
        }

        [TestMethod]
        public void TryAttemptWithRetryHandler()
        {
            int retryCount = 0;

            var succeeded = Attempter.Try(() =>
            {
                throw new Exception();
            }, 30, attempt => retryCount++);

            Assert.AreEqual(retryCount, 29);
            Assert.IsFalse(succeeded);
        }

        [TestMethod]
        public void TryAttemptWithRetryHandlerCancel()
        {
            int attemptCount = 0;

            var succeeded = Attempter.Try(() =>
            {
                attemptCount++;
                throw new Exception();
            }, 30, attempt => attempt.Cancelled = true);

            Assert.AreEqual(attemptCount, 1);
            Assert.IsFalse(succeeded);
        }

        [TestMethod]
        public void TryAttemptWithRetryHandlerOverride()
        {
            int value = 0;
            int attemptCount = 0;

            var succeeded = Attempter.Try(() =>
            {
                attemptCount++;
                value = 1;
                throw new Exception();
            }, 30, attempt =>
            {
                value = -1;
                attempt.Succeeded = true;
                attempt.Handled = true;
            });

            Assert.AreEqual(value, -1);
            Assert.AreEqual(attemptCount, 1);
            Assert.IsTrue(succeeded);
        }
    }
}
