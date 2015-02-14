using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Attemptation;

namespace AttemptationUnitTests
{
    [TestClass]
    public class AttempterStaticGetTests
    {
        [TestMethod]
        public void TryReturnsFalseOnFailedValueGet()
        {
            int value;
            var succeeded = Attempter.Try(() =>
            {
                throw new Exception();
            }, out value);

            Assert.IsFalse(succeeded);
            Assert.AreEqual(value, 0);
        }

        [TestMethod]
        public void TryReturnsFalseOnFailedObjectGet()
        {
            string value;
            var succeeded = Attempter.Try(() =>
            {
                throw new Exception();
            }, out value);

            Assert.IsFalse(succeeded);
            Assert.IsNull(value);
        }

        [TestMethod]
        public void TryReturnsTrueAndResultOnSuceededValueGet()
        {
            int value;
            var succeeded = Attempter.Try(() =>
            {
                return 10;
            }, out value);

            Assert.IsTrue(succeeded);
            Assert.AreEqual(value, 10);
        }

        [TestMethod]
        public void TryReturnsTrueAndResultOnSuceededObjectGet()
        {
            string value;
            var succeeded = Attempter.Try(() =>
            {
                return "test-string";
            }, out value);

            Assert.IsTrue(succeeded);
            Assert.AreEqual(value, "test-string");
        }

        [TestMethod]
        public void TryAttemptsOnceByDefaultForGet()
        {
            int attemptCount = 0;
            string value;

            var succeeded = Attempter.Try(() =>
            {
                attemptCount++;
                throw new Exception();
            }, out value);

            Assert.AreEqual(attemptCount, 1);
            Assert.IsFalse(succeeded);
            Assert.IsNull(value);
        }

        [TestMethod]
        public void TryAttemptsBasedOnParameterForGet()
        {
            int attemptCount = 0;
            string value;

            var succeeded = Attempter.Try(() =>
            {
                attemptCount++;
                throw new Exception();
            }, out value, 10);

            Assert.AreEqual(attemptCount, 10);
            Assert.IsFalse(succeeded);
            Assert.IsNull(value);
        }

        [TestMethod]
        public void TryAttemptWithRetryHandlerForGet()
        {
            int retryCount = 0;
            string value;

            var succeeded = Attempter.Try(() =>
            {
                throw new Exception();
            }, out value, 30, attempt => retryCount++);

            Assert.AreEqual(retryCount, 29);
            Assert.IsFalse(succeeded);
            Assert.IsNull(value);
        }

        [TestMethod]
        public void TryAttemptWithRetryHandlerCancelForGet()
        {
            int attemptCount = 0;
            string value;

            var succeeded = Attempter.Try(() =>
            {
                attemptCount++;
                throw new Exception();
            }, out value, 30, attempt => attempt.Cancelled = true);

            Assert.AreEqual(attemptCount, 1);
            Assert.IsFalse(succeeded);
            Assert.IsNull(value);
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
            }, out value, 30, attempt =>
            {
                attempt.Result = -1;
                attempt.Succeeded = true;
                attempt.Handled = true;
            });

            Assert.AreEqual(-1, value);
            Assert.AreEqual(attemptCount, 1);
            Assert.IsTrue(succeeded);
        }
    }
}
