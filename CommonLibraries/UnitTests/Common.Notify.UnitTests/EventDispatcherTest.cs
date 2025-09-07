namespace Common.Notify.UnitTests
{
    using System;
    using System.Threading;
    using Microsoft.Extensions.Logging;

    using NUnit.Framework;
    using Moq;

    using Common.Notify;

    [TestFixture]
    public class EventDispatcherTest
    {
        [Test]
        public void TestStartStop()
        {
            EventDispatcher eventDispatcher = new EventDispatcher(null, "test");
            Assert.That(eventDispatcher.Name, Does.StartWith("test:"));
            eventDispatcher.Dispose();
            eventDispatcher.Enqueue(() => { });
        }

        [Test]
        public void TestEnqueue()
        {
            EventDispatcher eventDispatcher = new EventDispatcher(null, "test");
            eventDispatcher.Enqueue(Increase);
            Thread.Sleep(50);
            Assert.That(_count, Is.EqualTo(1));
            eventDispatcher.Dispose();

        }
        private int _count = 0;
        private void Increase()
        {
            Interlocked.Increment(ref _count);
        }

        [Test]
        public void TestStopWithRunningTask()
        {
            EventDispatcher eventDispatcher = new EventDispatcher(null, "test");
            eventDispatcher.Enqueue(IncreaseWait);
            Thread.Sleep(20);
            eventDispatcher.Enqueue(IncreaseWait);
            eventDispatcher.Enqueue(IncreaseWait);
            eventDispatcher.Enqueue(IncreaseWait);
            Thread.Sleep(20);
            eventDispatcher.Dispose();
            Thread.Sleep(30);
            Assert.That(_countWait, Is.GreaterThanOrEqualTo(2));
        }
        private int _countWait = 0;
        private void IncreaseWait()
        {
            Interlocked.Increment(ref _countWait);
            Thread.Sleep(30);
        }
        [Test]
        public void TestNotErrorIfActionException()
        {
            Assert.DoesNotThrow(() =>
            {
                EventDispatcher eventDispatcher = new EventDispatcher(null, "test");
                eventDispatcher.Enqueue(() => throw new Exception());
                Thread.Sleep(50);
                eventDispatcher.Dispose();
            });
        }
        [Test]
        public void TestNotErrorIfActionExceptionWithLogger()
        {
            Mock<ILogger> log = new Mock<ILogger>(MockBehavior.Strict);
            log.Setup(l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => true), It.IsAny<Exception>(), It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true))).Verifiable(); 
            Assert.DoesNotThrow(() =>
            {
                EventDispatcher eventDispatcher = new EventDispatcher(log.Object, "test");
                eventDispatcher.Enqueue(() => throw new Exception());
                Thread.Sleep(50);
                eventDispatcher.Dispose();
            });
            log.Verify();
        }
        [Test]
        public void TestErrorRaiseIfGlobalExceptionStopThread()
        {
            Mock<ILogger> log = new Mock<ILogger>(MockBehavior.Strict);
            log.Setup(l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => true), It.IsAny<Exception>(), It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true))).Throws(new Exception());
            Assert.Throws<Exception>(() =>
            {
                EventDispatcher eventDispatcher = new EventDispatcher(log.Object, "test");
                eventDispatcher.Enqueue(() => throw new Exception());
                Thread.Sleep(50);
                eventDispatcher.Dispose();
            });
        }
    }
}
