using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Attemptation;
using System.Threading.Tasks;

namespace AttemptationUnitTests
{
    [TestClass]
    public class AttempterStaticActionAsyncTests
    {
        [TestMethod]
        public async Task TryReturnsFalseOnFailedActionAsync()
        {
            var succeeded = await Attempter.TryAsync(() => {
                throw new Exception();
            });

            Assert.IsFalse(succeeded);
        }

        [TestMethod]
        public async Task TryReturnsTrueOnSucceededActionAsync()
        {
            var succeeded = await Attempter.TryAsync(async () =>
            {
                await Task.Delay(1000);
            });

            Assert.IsTrue(succeeded);
        }

        [TestMethod]
        public async Task TryAttemptsOnceByDefaultAsync()
        {
            int attemptCount = 0;

            var succeeded = await Attempter.TryAsync(() =>
            {
                attemptCount++;
                throw new Exception();
            });

            Assert.AreEqual(attemptCount, 1);
            Assert.IsFalse(succeeded);
        }

        [TestMethod]
        public async Task TryAttemptsBasedOnParameterAsync()
        {
            int attemptCount = 0;

            var succeeded = await Attempter.TryAsync(() =>
            {
                attemptCount++;
                throw new Exception();
            }, 10);

            Assert.AreEqual(10, attemptCount);
            Assert.IsFalse(succeeded);
        }

        [TestMethod]
        public async Task TryAttemptWithRetryHandlerAsync()
        {
            int retryCount = 0;

            var succeeded = await Attempter.TryAsync(() =>
            {
                throw new Exception();
            }, 30, attempt => {
                retryCount++;
                return Task.FromResult(false);
            });

            Assert.AreEqual(29, retryCount);
            Assert.IsFalse(succeeded);
        }

        [TestMethod]
        public async Task TryAttemptWithRetryHandlerCancelAsync()
        {
            int attemptCount = 0;

            var succeeded = await Attempter.TryAsync(() =>
            {
                attemptCount++;
                throw new Exception();
            }, 30, attempt => {
                attempt.Cancelled = true;
                return Task.FromResult(false);
            });

            Assert.AreEqual(attemptCount, 1);
            Assert.IsFalse(succeeded);
        }

        [TestMethod]
        public async Task TryAttemptWithRetryHandlerOverrideAsync()
        {
            int value = 0;
            int attemptCount = 0;

            var succeeded = await Attempter.TryAsync(() =>
            {
                attemptCount++;
                value = 1;
                throw new Exception();
            }, 30, attempt =>
            {
                value = -1;
                attempt.Succeeded = true;
                attempt.Handled = true;

                return Task.FromResult(false);
            });

            Assert.AreEqual(value, -1);
            Assert.AreEqual(attemptCount, 1);
            Assert.IsTrue(succeeded);
        }
    }
}
