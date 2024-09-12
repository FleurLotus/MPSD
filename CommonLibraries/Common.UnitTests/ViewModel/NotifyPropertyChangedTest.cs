namespace Common.UnitTests.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Common.ViewModel;

    using NUnit.Framework;

    [TestFixture]
    public class NotifyPropertyChangedTest
    {
        private ViewModel _vm;
        private readonly Collection<string> _notified = new Collection<string>();

        [SetUp]
        public void SetUp()
        {
            _notified.Clear();
            _vm = new ViewModel();
            _vm.PropertyChanged += PropertyChanged;
        }

        [TearDown]
        public void TearDown()
        {
            if (_vm != null)
            {
                _vm.PropertyChanged -= PropertyChanged;
            }
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
            Assert.That(_notified.Count == 0, "Not empty collection");
            _vm.Property1 = "a";
            Assert.That(_notified.Count == 1, "not the expected number of notification after Property1 set");
            Assert.That(_notified[^1] == "Property1", "not the expected notification after Property1 set");
            _vm.Property4 = "a";
            Assert.That(_notified.Count == 2, "not the expected number of notification after property4 set");
            Assert.That(_notified[^1] == "Property4", "not the expected notification after Property4 set");
            _vm.Property7 = "a";
            Assert.That(_notified.Count == 3, "not the expected number of notification after Property7 set");
            Assert.That(_notified[^1] == "Property7", "not the expected notification after Property7 set");
        }
        [Test]
        public void TestWithLink()
        {
            _vm.InitLink();
            Assert.That(_notified.Count == 0, "Not empty collection");
            _vm.Property1 = "a";
            Assert.That(_notified.Count == 3, "not the expected number of notification after Property1 set");
            Assert.That(_notified[^3] == "Property1", "not the expected notification after Property1 set");
            Assert.That(_notified[^2] == "Property2", "not the expected notification after Property1 set");
            Assert.That(_notified[^1] == "Property3", "not the expected notification after Property1 set");
            _vm.Property4 = "a";
            Assert.That(_notified.Count == 6, "not the expected number of notification after Property4 set");
            Assert.That(_notified[^3] == "Property4", "not the expected notification after Property4 set");
            Assert.That(_notified[^2] == "Property5", "not the expected notification after Property4 set");
            Assert.That(_notified[^1] == "Property6", "not the expected notification after Property4 set");
            _vm.Property7 = "a";
            Assert.That(_notified.Count == 8, "not the expected number of notification after Property7 set");
            Assert.That(_notified[^2] == "Property7", "not the expected notification after Property7 set");
            Assert.That(_notified[^1] == "Property8", "not the expected notification after Property7 set");
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _notified.Add(e.PropertyName);
        }

        //Used by reflection
        // ReSharper disable UnusedMember.Local
        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private class ViewModel : NotifyPropertyChangedBase
        {
            public void InitLink()
            {
                //Test Chaining
                AddLinkedProperty(nameof(Property1), nameof(Property2));
                AddLinkedProperty(nameof(Property2), nameof(Property3));

                //Test multiple child
                AddLinkedProperty(nameof(Property4), new string[] { nameof(Property5), nameof(Property6) });

                //Test no cycle
                AddLinkedProperty(nameof(Property7), nameof(Property8));
                AddLinkedProperty(nameof(Property8), nameof(Property7));
            }
            public void InitLinkDuplicate()
            {
                AddLinkedProperty(nameof(Property1), nameof(Property1));
            }
            public void InitLinkUnknownSource()
            {
                //Unknown because only instance | public property are allowed
                AddLinkedProperty(nameof(Inner), nameof(Property1));
            }
            public void InitLinkUnknownDestination()
            {
                //Unknown because only instance | public property are allowed
                AddLinkedProperty(nameof(Property1), nameof(Inner));
            }

            public static string Inner { get; set; }

            private string _property1;
            private string _property2;
            private string _property3;
            private string _property4;
            private string _property5;
            private string _property6;
            private string _property7;
            private string _property8;

            public string Property8
            {
                get { return _property8; }

                set
                {
                    if (value != _property8)
                    {
                        _property8 = value;
                        OnNotifyPropertyChanged(nameof(Property8));
                    }
                }
            }
            public string Property7
            {
                get { return _property7; }
                set
                {
                    if (value != _property7)
                    {
                        _property7 = value;
                        OnNotifyPropertyChanged(nameof(Property7));
                    }
                }
            }
            public string Property6
            {
                get { return _property6; }
                set
                {
                    if (value != _property6)
                    {
                        _property6 = value;
                        OnNotifyPropertyChanged(nameof(Property6));
                    }
                }
            }
            public string Property5
            {
                get { return _property5; }
                set
                {
                    if (value != _property5)
                    {
                        _property5 = value;
                        OnNotifyPropertyChanged(nameof(Property5));
                    }
                }
            }
            public string Property4
            {
                get { return _property4; }
                set
                {
                    if (value != _property4)
                    {
                        _property4 = value;
                        OnNotifyPropertyChanged(nameof(Property4));
                    }
                }
            }
            public string Property3
            {
                get { return _property3; }
                set
                {
                    if (value != _property3)
                    {
                        _property3 = value;
                        OnNotifyPropertyChanged(nameof(Property3));
                    }
                }
            }

            public string Property2
            {
                get { return _property2; }
                set
                {
                    if (value != _property2)
                    {
                        _property2 = value;
                        OnNotifyPropertyChanged(nameof(Property2));
                    }
                }
            }
            public string Property1
            {
                get { return _property1; }
                set
                {
                    if (value != _property1)
                    {
                        _property1 = value;
                        OnNotifyPropertyChanged(nameof(Property1));
                    }
                }
            }
        }
        // ReSharper restore UnusedMember.Local
        // ReSharper restore MemberCanBePrivate.Local
        // ReSharper restore UnusedAutoPropertyAccessor.Local
    }
}