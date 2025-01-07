namespace Common.Library.UnitTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Common.Library.Threading;

    using NUnit.Framework;

    [TestFixture]
    public class SafeFireAndForgetExtensionsTest
    {
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        private readonly int Timeout = 30;

        [Test]
        public void TestSuccessfullTask()
        {
            Task t = CreateTask();
            t.FireAndForgetSafeAsync();
            Assert.That(_autoResetEvent.WaitOne(Timeout * 3), Is.True);
        }
        private async Task CreateTask()
        {
            await Task.Delay(Timeout);
            _autoResetEvent.Set();
        }
        [Test]
        public void TestFailureTask()
        {
            Task t = CreateFailureTask();
            t.FireAndForgetSafeAsync();
            Assert.That(t.IsCompleted, Is.False);
            Thread.Sleep(Timeout * 3);
            Assert.That(t.IsCompleted, Is.True);
        }
        private async Task CreateFailureTask()
        {
            await Task.Delay(Timeout);
            throw new Exception();
        }
        [Test]
        public void TestFailureTaskWithHandler()
        {
            Task t = CreateFailureTask();
            Handler h = new Handler();
            t.FireAndForgetSafeAsync(h);
            Assert.That(t.IsCompleted, Is.False);
            Thread.Sleep(Timeout * 3);
            Assert.That(t.IsCompleted, Is.True);
            Assert.That(h.Exception, Is.Not.Null);
        }
        private class Handler : IErrorHandler
        {
            public Exception Exception { get; private set; }

            public void HandleError(Exception ex)
            {
                Exception = ex;
            }
        }
    }
}
