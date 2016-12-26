using Common.TransientExceptionHandling;
using NUnit.Framework;
using System;

namespace Common.Tests.TransientExceptionHandling
{
    [TestFixture]
    public class RetryerTest
    {
        private readonly Retryer _retryer = new Retryer();

        [Test]
        public void ActionThatAlwaysThrowsShouldBeRetriedThreeTimes()
        {
            var callCount = 0;
            const int retryCount = 3;
            Assert.Throws<Exception>(() =>
            {
                _retryer.ExecuteWithRetry<Exception>(() =>
                {
                    callCount++;
                    throw new Exception();
                }, retryCount);
            });

            Assert.That(callCount, Is.EqualTo(retryCount));
        }

        [Test]
        public void ActionThatIsNotThrowingExceptionShouldBeInvokedOnlyOnce()
        {
            var callCount = 0;
            const int retryCount = 3;
            _retryer.ExecuteWithRetry<Exception>(() => callCount++, retryCount);

            Assert.That(callCount, Is.EqualTo(1));
        }

        [Test]
        public void ActionThatIsNotThrowingExceptionShouldBeInvokedOnlyOnceAndReturnCorrectResult()
        {
            var callCount = 0;
            const int retryCount = 3;
            var result = _retryer.ExecuteWithRetry<Exception, string>(() =>
            {
                callCount++;
                return "result!";
            }, retryCount);

            Assert.That(callCount, Is.EqualTo(1));
            Assert.That(result, Is.EqualTo("result!"));

        }

        [Test]
        public void DerivedExceptionsShouldBeRetried()
        {
            var callCount = 0;
            const int retryCount = 3;
            Assert.Throws<DerivedException>(() =>
            {
                _retryer.ExecuteWithRetry<BaseException>(() =>
                {
                    callCount++;
                    throw new DerivedException();
                }, retryCount);
            });

            Assert.That(callCount, Is.EqualTo(retryCount));
        }

        [Test]
        public void ActionThrowingExceptionThatIsNotTheRetryExceptionOrItsDerivativeShouldNotBeRetried()
        {
            var callCount = 0;
            const int retryCount = 3;
            Assert.Throws<NotDerivedException>(() =>
            {
                _retryer.ExecuteWithRetry<BaseException>(() =>
                {
                    callCount++;
                    throw new NotDerivedException();
                }, retryCount);
            });

            Assert.That(callCount, Is.EqualTo(1));
        }

        class BaseException : Exception
        { }

        class DerivedException : BaseException
        { }

        class NotDerivedException : Exception
        { }
    }
}
