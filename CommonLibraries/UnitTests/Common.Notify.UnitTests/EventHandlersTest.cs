namespace Common.Notify.UnitTests
{
    using System;

    using Common.Notify;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class EventHandlersTest
    {
        [Test]
        public void TestAddNull()
        {
            EventHandlers<EventArgs<object>> eventHandlers = new EventHandlers<EventArgs<object>>();
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("handler"), () => eventHandlers.Add(null));
        }
        [Test]
        public void TestAddAlreadyPresent()
        {
            EventHandlers<EventArgs<object>> eventHandlers = new EventHandlers<EventArgs<object>>();
            EventHandler<EventArgs<object>> handler = Dummy;
            eventHandlers.Add(handler);
            Assert.That(eventHandlers.Count, Is.EqualTo(1));
            Assert.Throws(Is.TypeOf<HandlerAlreadyKnownException>(), () => eventHandlers.Add(handler));
        }

        private void Dummy(object sender, EventArgs<object> args)
        {
        }
        private void Dummy2(object sender, EventArgs<object> args)
        {
        }
        [Test]
        public void TestRemoveNull()
        {
            EventHandlers<EventArgs<object>> eventHandlers = new EventHandlers<EventArgs<object>>();
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("handler"), () => eventHandlers.Remove(null));
        }

        [Test]
        public void TestRemoveNotPresent()
        {
            EventHandlers<EventArgs<object>> eventHandlers = new EventHandlers<EventArgs<object>>();
            EventHandler<EventArgs<object>> handler = Dummy;
            Assert.Throws(Is.TypeOf<HandlerNotKnownException>(), () => eventHandlers.Remove(handler));
        }

        [Test]
        public void TestAddRemove()
        {
            EventHandlers<EventArgs<object>> eventHandlers = new EventHandlers<EventArgs<object>>();
            EventHandler<EventArgs<object>> handler = Dummy;
            EventHandler<EventArgs<object>> handler2 = Dummy2;
            Assert.That(eventHandlers.Count, Is.EqualTo(0));
            eventHandlers.Add(handler);
            Assert.That(eventHandlers.Count, Is.EqualTo(1));
            eventHandlers.Add(handler2);
            Assert.That(eventHandlers.Count, Is.EqualTo(2));
            eventHandlers.Remove(handler);
            Assert.That(eventHandlers.Count, Is.EqualTo(1));
            eventHandlers.Remove(handler2);
            Assert.That(eventHandlers.Count, Is.EqualTo(0));
        }
        [Test]
        public void TestAddRemoveWithCallBack()
        {
            EventHandler<EventArgs<object>> handler = Dummy;

            HandlerCheck handlerCheckAdding = new HandlerCheck(handler, 0);
            HandlerCheck handlerCheckAdded = new HandlerCheck(handler, 1);
            HandlerCheck handlerCheckRemoving = new HandlerCheck(handler, 1);
            HandlerCheck handlerCheckRemoved = new HandlerCheck(handler, 0);

            EventHandlers<EventArgs<object>> eventHandlers = new EventHandlers<EventArgs<object>>(handlerCheckAdding.Callback, handlerCheckAdded.Callback, handlerCheckRemoving.Callback, handlerCheckRemoved.Callback);
            eventHandlers.Add(handler);
            Assert.That(handlerCheckAdding.CallCount, Is.EqualTo(1));
            Assert.That(handlerCheckAdded.CallCount, Is.EqualTo(1));
            Assert.That(handlerCheckRemoving.CallCount, Is.EqualTo(0));
            Assert.That(handlerCheckRemoved.CallCount, Is.EqualTo(0));
            eventHandlers.Remove(handler);
            Assert.That(handlerCheckAdding.CallCount, Is.EqualTo(1));
            Assert.That(handlerCheckAdded.CallCount, Is.EqualTo(1));
            Assert.That(handlerCheckRemoving.CallCount, Is.EqualTo(1));
            Assert.That(handlerCheckRemoved.CallCount, Is.EqualTo(1));
        }
        private class HandlerCheck
        {
            private readonly EventHandler<EventArgs<object>> _expectedHandler;
            private readonly int _expectedCount;

            public HandlerCheck(EventHandler<EventArgs<object>> expectedHandler, int expectedCount)
            {
                _expectedHandler = expectedHandler;
                _expectedCount = expectedCount;
            }

            public int CallCount { get; private set; }

            public void Callback(EventHandler<EventArgs<object>> handler, int count)
            {
                Assert.That(count, Is.EqualTo(_expectedCount));
                Assert.That(handler, Is.EqualTo(_expectedHandler));
                CallCount++;
            }
        }

        [Test]
        public void TestClear()
        {
            EventHandlers<EventArgs<object>> eventHandlers = new EventHandlers<EventArgs<object>>();
            EventHandler<EventArgs<object>> handler = Dummy;
            EventHandler<EventArgs<object>> handler2 = Dummy2;
            eventHandlers.Add(handler);
            eventHandlers.Add(handler2);
            Assert.That(eventHandlers.Count, Is.EqualTo(2));
            eventHandlers.Clear();
            Assert.That(eventHandlers.Count, Is.EqualTo(0));
        }
        [Test]
        public void TestClearWithCallBack()
        {
            EventHandler<EventArgs<object>> handler = Dummy;

            HandlerCheck handlerCheckRemoving = new HandlerCheck(handler, 0);
            HandlerCheck handlerCheckRemoved = new HandlerCheck(handler, 0);

            EventHandlers<EventArgs<object>> eventHandlers = new EventHandlers<EventArgs<object>>(null, null, handlerCheckRemoving.Callback, handlerCheckRemoved.Callback);
            eventHandlers.Add(handler);
            Assert.That(handlerCheckRemoving.CallCount, Is.EqualTo(0));
            Assert.That(handlerCheckRemoved.CallCount, Is.EqualTo(0));
            eventHandlers.Clear();
            Assert.That(handlerCheckRemoving.CallCount, Is.EqualTo(0));
            Assert.That(handlerCheckRemoved.CallCount, Is.EqualTo(0));
        }

        [Test]
        public void TestNotify()
        {
            object sender = new object();
            EventArgs<object> args = new EventArgs<object>(new object());

            object calledSender = null;
            EventArgs<object> calledArgs = null;

            EventHandlers<EventArgs<object>> eventHandlers = new EventHandlers<EventArgs<object>>();
            eventHandlers.Add((s, a) =>
            {
                calledSender = s;
                calledArgs = a;
            });

            Mock<IEventDispatcher> eventDispatcher = new Mock<IEventDispatcher>(MockBehavior.Strict);
            eventDispatcher.Setup(ed => ed.Enqueue(It.IsAny<Action>())).Callback((Action a) => a()).Verifiable();
            eventHandlers.Notify(eventDispatcher.Object, sender, args);

            Assert.That(calledSender, Is.EqualTo(sender));
            Assert.That(calledArgs, Is.EqualTo(args));

            eventDispatcher.VerifyAll();
        }
        [Test]
        public void TestNotifyDoesNotThrow()
        {
            object sender = new object();
            EventArgs<object> args = new EventArgs<object>(new object());

            EventHandlers<EventArgs<object>> eventHandlers = new EventHandlers<EventArgs<object>>();
            eventHandlers.Add((s, a) => throw new Exception("handler exception"));

            Mock<IEventDispatcher> eventDispatcher = new Mock<IEventDispatcher>(MockBehavior.Strict);
            eventDispatcher.Setup(ed => ed.Enqueue(It.IsAny<Action>())).Callback((Action a) => a()).Verifiable();
            eventHandlers.Notify(eventDispatcher.Object, sender, args);
            eventDispatcher.VerifyAll();
        }
        [Test]
        public void TestNotifyWithExceptioncallBack()
        {
            object sender = new object();
            EventArgs<object> args = new EventArgs<object>(new object());

            EventHandler<EventArgs<object>> calledHandler = null;
            Exception calledException = null;
            Action<EventHandler<EventArgs<object>>, Exception> exceptionCallback = (h, e) =>
            {
                calledHandler = h;
                calledException = e;
            };

            Exception ex = new Exception("handler exception");
            EventHandlers<EventArgs<object>> eventHandlers = new EventHandlers<EventArgs<object>>();
            EventHandler<EventArgs<object>> handler = (s, a) =>
            {
                throw ex;
            };

            eventHandlers.Add(handler);

            Mock<IEventDispatcher> eventDispatcher = new Mock<IEventDispatcher>(MockBehavior.Strict);
            eventDispatcher.Setup(ed => ed.Enqueue(It.IsAny<Action>())).Callback((Action a) => a()).Verifiable();
            eventHandlers.Notify(eventDispatcher.Object, sender, args, exceptionCallback);

            Assert.That(calledHandler, Is.EqualTo(handler));
            Assert.That(calledException, Is.EqualTo(ex));

            eventDispatcher.VerifyAll();
        }
    }
}