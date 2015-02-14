using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Attemptation;
using System.Threading.Tasks;

namespace AttemptationUnitTests
{
    [TestClass]
    public class AttempterStaticGetAsyncTests
    {
        [TestMethod]
        public async Task TryReturnsFalseOnFailedValueGetAsync()
        {
            var isFalse = false;

            var result = await Attempter.TryAsync(() =>
            {
                if (isFalse)
                    return Task.FromResult(10);

                throw new Exception();
            });

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(result.Result, 0);
        }

        [TestMethod]
        public async Task TryReturnsFalseOnFailedObjectGetAsync()
        {
            var isFalse = false;

            var result = await Attempter.TryAsync(() =>
            {
                if (isFalse)
                    return Task.FromResult("test-string");

                throw new Exception();
            });

            Assert.IsFalse(result.Succeeded);
            Assert.IsNull(result.Result);
        }

        [TestMethod]
        public async Task TryReturnsTrueAndResultOnSuceededValueGetAsync()
        {
            var result = await Attempter.TryAsync(() =>
            {
                return Task.FromResult(10);
            });

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(result.Result, 10);
        }

        [TestMethod]
        public async Task TryReturnsTrueAndResultOnSuceededObjectGetAsync()
        {
            var result = await Attempter.TryAsync(() =>
            {
                return Task.FromResult("test-string");
            });

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(result.Result, "test-string");
        }

        [TestMethod]
        public async Task TryAttemptsOnceByDefaultForGetAsync()
        {
            var isFalse = false;
            int attemptCount = 0;

            var result = await Attempter.TryAsync(() =>
            {
                attemptCount++;

                if (isFalse)
                    return Task.FromResult("test-string");

                throw new Exception();
            });

            Assert.AreEqual(attemptCount, 1);
            Assert.IsFalse(result.Succeeded);
            Assert.IsNull(result.Result);
        }

        [TestMethod]
        public async Task TryAttemptsBasedOnParameterForGetAsync()
        {
            var isFalse = false;
            int attemptCount = 0;

            var result = await Attempter.TryAsync(() =>
            {
                attemptCount++;

                if (isFalse)
                    return Task.FromResult("test-string");

                throw new Exception();
            }, 10);

            Assert.AreEqual(attemptCount, 10);
            Assert.IsFalse(result.Succeeded);
            Assert.IsNull(result.Result);
        }

        [TestMethod]
        public async Task TryAttemptWithRetryHandlerForGetAsync()
        {
            var isFalse = false;
            int retryCount = 0;

            var result = await Attempter.TryAsync(() =>
            {
                if (isFalse)
                    return Task.FromResult("test-string");

                throw new Exception();
            }, 30, attempt =>
            {
                retryCount++;
                return Task.FromResult(false);
            });

            Assert.AreEqual(retryCount, 29);
            Assert.IsFalse(result.Succeeded);
            Assert.IsNull(result.Result);
        }

        [TestMethod]
        public async Task TryAttemptWithRetryHandlerCancelForGetAsync()
        {
            int attemptCount = 0;
            var isFalse = false;

            var result = await Attempter.TryAsync(() =>
            {
                attemptCount++;

                if (isFalse)
                    return Task.FromResult("test-string");

                throw new Exception();
            }, 30, attempt =>
            {
                attempt.Cancelled = true;
                return Task.FromResult(false);
            });

            Assert.AreEqual(attemptCount, 1);
            Assert.IsFalse(result.Succeeded);
            Assert.IsNull(result.Result);
        }

        [TestMethod]
        public async Task TryAttemptWithRetryHandlerOverrideAsync()
        {
            int attemptCount = 0;
            var isFalse = false;

            var result = await Attempter.TryAsync(() =>
            {
                attemptCount++;
                if (isFalse)
                    return Task.FromResult(1);

                throw new Exception();
            }, 30, attempt =>
            {
                attempt.Result = -1;
                attempt.Succeeded = true;
                attempt.Handled = true;
                return Task.FromResult(false);
            });

            Assert.AreEqual(-1, result.Result);
            Assert.AreEqual(attemptCount, 1);
            Assert.IsTrue(result.Succeeded);
        }
    }
}
