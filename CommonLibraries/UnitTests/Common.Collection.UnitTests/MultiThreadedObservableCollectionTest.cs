namespace Common.Collection.UnitTests
{
    using System;

    using NUnit.Framework;
    using Moq;

    using Common.Library;

    [TestFixture]
    public class MultiThreadedObservableCollectionTest
    {
        [Test]
        public void TestNoDispatcherNoError()
        {
            Assert.DoesNotThrow(() =>
            {
                MultiThreadedObservableCollection<int> collection = new MultiThreadedObservableCollection<int>(null)
                {
                    1,
                    2,
                    3
                };
                Assert.That(collection.Count, Is.EqualTo(3), "Not the good count");
            }, "No dispatcher should not throw an error");
        }
        [Test]
        public void TestNoDispatcherNoErrorEvenWithHandler()
        {
            Assert.DoesNotThrow(() =>
            {
                MultiThreadedObservableCollection<int> collection = new MultiThreadedObservableCollection<int>(null);
                collection.CollectionChanged += (s, e) => { };
                collection.Add(1);
                collection.Add(2);
                collection.Add(3);
                Assert.That(collection.Count, Is.EqualTo(3), "Not the good count");
            }, "No dispatcher should not throw an error even with a handler");

        }
        [Test]
        public void TestWithDispatcherNoError()
        {
            Mock<IDispatcherInvoker> mockDispatcher = new Mock<IDispatcherInvoker>(MockBehavior.Strict);

            Assert.DoesNotThrow(() =>
            {
                MultiThreadedObservableCollection<int> collection = new MultiThreadedObservableCollection<int>(mockDispatcher.Object)
                {
                    1,
                    2,
                    3
                };
                Assert.That(collection.Count, Is.EqualTo(3), "Not the good count");
            }, "With dispatcher should not throw an error");
        }

        [Test]
        public void TestWithDispatcherAndHandler()
        {
            Mock<IDispatcherInvoker> mockDispatcher = new Mock<IDispatcherInvoker>(MockBehavior.Strict);
            mockDispatcher.Setup(d => d.Invoke(It.IsAny<Action>())).Callback<Action>(a => a());

            int countcall = 0;
            int countcall2 = 0;

            MultiThreadedObservableCollection<int> collection = new MultiThreadedObservableCollection<int>(mockDispatcher.Object);
            collection.CollectionChanged += (s, e) => 
            {
                Assert.That(s, Is.EqualTo(collection), "Sender is not the expected one");
                countcall++;
            };

            collection.Add(1);
            collection.CollectionChanged += (s, e) =>
            {
                Assert.That(s, Is.EqualTo(collection), "Sender is not the expected one");
                countcall2++;
            };

            collection.Add(2);
            collection.Add(3);
            Assert.That(collection.Count, Is.EqualTo(3), "Not the good count");
            Assert.That(countcall, Is.EqualTo(3), "Not the good count of call back");
            Assert.That(countcall2, Is.EqualTo(2), "Not the good count of call back2");
        }
    }
}
