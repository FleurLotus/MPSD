namespace Common.Library.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    using NUnit.Framework;

    using Common.Library.Collection;

    [TestFixture]
    public class RangeObservableCollectionTest
    {
        [Test]
        public void TestAddRangeNull()
        {
            RangeObservableCollection<object> obs = new RangeObservableCollection<object>();
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("list"), () => obs.AddRange(null));
        }

        [Test]
        public void TestNotifyUnitary()
        {
            RangeObservableCollection<object> obs = new RangeObservableCollection<object>(new[] { new object() });
            int counter = 0;

            NotifyCollectionChangedEventHandler h = (o,h) => counter++;
            obs.CollectionChanged += h;

            for (int i = 0; i < 10; i++)
            {
                obs.Add(new object());
            }

            Assert.That(counter, Is.EqualTo(10));
        }

        [Test]
        public void TestNotifyRange()
        {
            RangeObservableCollection<object> obs = new RangeObservableCollection<object>(new[] { new object() });
            int counter = 0;

            NotifyCollectionChangedEventHandler h = (o, h) => counter++;
            obs.CollectionChanged += h;

            List<object> l = new List<object>();
            for (int i = 0; i < 10; i++)
            {
                l.Add(new object());
            }

            obs.AddRange(l);

            Assert.That(counter, Is.EqualTo(1));
        }
    }
}
