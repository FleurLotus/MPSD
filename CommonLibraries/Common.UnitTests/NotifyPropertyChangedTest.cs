namespace Common.UnitTests
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using NUnit.Framework;

    [TestFixture]
    public class NotifyPropertyChangedTest
    {
        private TestViewModel _vm;
        private readonly Collection<string> _notified = new Collection<string>();
        
        [SetUp]
        public void SetUp()
        {
            _notified.Clear();
            _vm = new TestViewModel();
            _vm.PropertyChanged += PropertyChanged;

        }

        [TearDown]
        public void TearDown()
        {
            if (_vm != null)
                _vm.PropertyChanged -= PropertyChanged;
        }

        [Test]
        public void TestUnknownSource()
        {
            Assert.Throws<ArgumentException>(() => _vm.InitLinkUnknownSource(), "Unknown source must throw ArgumentException");
        }
        [Test]
        public void TestUnknownDestination()
        {
            Assert.Throws<ArgumentException>(() => _vm.InitLinkUnknownDestination(), "Unknown destination must throw ArgumentException");
        }
        [Test]
        public void TestSameSourceDestination()
        {
            Assert.Throws<ArgumentException>(() => _vm.InitLinkDuplicate(), "Same source and destination must throw ArgumentException");
        }

        [Test]
        public void TestWithNoLink()
        {
            Assert.IsTrue(_notified.Count == 0, "Not empty collection");
            _vm.Property1 = "a";
            Assert.IsTrue(_notified.Count == 1, "not the expected number of notification after Property1 set");
            Assert.IsTrue(_notified[_notified.Count - 1] == "Property1", "not the expected notification after Property1 set");
            _vm.Property4 = "a";
            Assert.IsTrue(_notified.Count == 2, "not the expected number of notification after property4 set");
            Assert.IsTrue(_notified[_notified.Count - 1] == "Property4", "not the expected notification after Property4 set");
            _vm.Property7 = "a";
            Assert.IsTrue(_notified.Count == 3, "not the expected number of notification after Property7 set");
            Assert.IsTrue(_notified[_notified.Count - 1] == "Property7", "not the expected notification after Property7 set");
        }
        [Test]
        public void TestWithLink()
        {
            _vm.InitLink();
            Assert.IsTrue(_notified.Count == 0, "Not empty collection");
            _vm.Property1 = "a";
            Assert.IsTrue(_notified.Count == 3, "not the expected number of notification after Property1 set");
            Assert.IsTrue(_notified[_notified.Count - 3] == "Property1", "not the expected notification after Property1 set");
            Assert.IsTrue(_notified[_notified.Count - 2] == "Property2", "not the expected notification after Property1 set");
            Assert.IsTrue(_notified[_notified.Count - 1] == "Property3", "not the expected notification after Property1 set");
            _vm.Property4 = "a";
            Assert.IsTrue(_notified.Count == 6, "not the expected number of notification after Property4 set");
            Assert.IsTrue(_notified[_notified.Count - 3] == "Property4", "not the expected notification after Property4 set");
            Assert.IsTrue(_notified[_notified.Count - 2] == "Property5", "not the expected notification after Property4 set");
            Assert.IsTrue(_notified[_notified.Count - 1] == "Property6", "not the expected notification after Property4 set");
            _vm.Property7 = "a";
            Assert.IsTrue(_notified.Count == 8, "not the expected number of notification after Property7 set");
            Assert.IsTrue(_notified[_notified.Count - 2] == "Property7", "not the expected notification after Property7 set");
            Assert.IsTrue(_notified[_notified.Count - 1] == "Property8", "not the expected notification after Property7 set");
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _notified.Add(e.PropertyName);
        }

    }
}
