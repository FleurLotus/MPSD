namespace Common.Collection.UnitTests
{
    using System.Collections.Specialized;
    using System.Threading;

    using Common.Collection;

    using NUnit.Framework;

    [TestFixture]
    public class AsyncObservableCollectionTest
    {
        [Test]
        public void TestAddItemsRaisesCollectionChanged()
        {
            AsyncObservableCollection<int> collection = new AsyncObservableCollection<int>();
            bool eventRaised = false;
            collection.CollectionChanged += (s, e) =>
            {
                eventRaised = true;
                Assert.That(e.Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
                Assert.That(e.NewItems[0], Is.EqualTo(42));
            };

            collection.Add(42);

            Assert.That(eventRaised, Is.True, "CollectionChanged event should be raised on Add.");
        }

        [Test]
        public void TestRemoveItemsRaisesCollectionChanged()
        {
            AsyncObservableCollection<int> collection = new AsyncObservableCollection<int> { 1, 2, 3 };
            bool eventRaised = false;
            collection.CollectionChanged += (s, e) =>
            {
                eventRaised = true;
                Assert.That(e.Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
                Assert.That(e.OldItems[0], Is.EqualTo(2));
            };

            collection.Remove(2);

            Assert.That(eventRaised, Is.True, "CollectionChanged event should be raised on Remove.");
        }

        [Test]
        public void TestCollectionChangedRaisedOnOtherThread()
        {
            SynchronizationContext context1 = new SynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(context1);

            AsyncObservableCollection<int> collection = new AsyncObservableCollection<int>(new int[] { 1, 2, 3 });
            bool eventRaised = false;
            ManualResetEvent evt = new ManualResetEvent(false);

            collection.CollectionChanged += (s, e) =>
            {
                eventRaised = true;
                evt.Set();
            };

            Thread thread = new Thread(() =>
            {
                SynchronizationContext context2 = new SynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(context2);
                collection.Add(99);
            });
            thread.Start();

            // Wait for the event to be raised (with timeout)
            evt.WaitOne(100);

            Assert.That(eventRaised, Is.True, "CollectionChanged should be raised even when modified from another thread.");
        }
    }
}