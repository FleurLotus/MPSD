namespace Common.ViewModel.UnitTests
{
    using System;
    using System.Collections.Generic;

    using Common.ViewModel.Validation;

    using NUnit.Framework;

    [TestFixture]
    public class ValidationUsingRulesTest
    {
        [Test]
        public void TestUnknownProperty()
        {
            Assert.Throws(Is.TypeOf<ArgumentException>().With.Message.StartsWith("property is unknown"), () => (new ViewModelWithValidation()).AddRuleWithUnknownSource(), "Unknown source must throw ArgumentException");
        }
        [Test]
        public void TestNullRuleList()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("rules"), () => (new ViewModelWithValidation()).AddRuleWithIEnumerableFuncNullRule(), "Null rules must throw ArgumentNullException");
        }
        [Test]
        public void TestNullNameList()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("propertyNames"), () => (new ViewModelWithValidation()).AddRuleWithIEnumerableNameNullRule(), "Null propertyNames must throw ArgumentNullException");
        }
        [Test]
        public void TestNullRule()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("rule"), () => (new ViewModelWithValidation()).AddRuleWithNullRule(), "Null rule must throw ArgumentNullException");
        }
        [Test]
        public void TestRule()
        {
            ViewModelWithValidation vm = new ViewModelWithValidation();

            //PropertyPublic and PropertyWithProtectedGet are null
            Assert.That(string.IsNullOrEmpty(vm.Error), Is.False, "Must have error");
            vm.PropertyPublic = "ahahah";
            //PropertyWithProtectedGet is null
            Assert.That(string.IsNullOrEmpty(vm.Error), Is.False, "Must still have error");
            vm.PropertyWithProtectedGet = "bbbb";
            //Ok
            Assert.That(string.IsNullOrEmpty(vm.Error), Is.True, "Must have not error");
            vm.PropertyWithNoRule = "bbbb";
            //No change for rules
            Assert.That(string.IsNullOrEmpty(vm.Error), Is.True, "Must still have not error");
            vm.PropertyPublic = "";
            //PropertyPublic is empty
            Assert.That(string.IsNullOrEmpty(vm.Error), Is.False, "Must have error again");

        }
        [Test]
        public void TestRuleWithChild()
        {
            ViewModelWithValidation2 vm = new ViewModelWithValidation2();

            //PropertyPublic is empty and child not set
            Assert.That(string.IsNullOrEmpty(vm.Error), Is.False, "Must have error");
            vm.PropertyPublic = "ahahah";
            //OK
            Assert.That(string.IsNullOrEmpty(vm.Error), Is.True, "Must have not error");
            vm.CreateChild();
            //Set child  and Child.PropertyPublic and Child.PropertyWithProtectedGet are null
            Assert.That(string.IsNullOrEmpty(vm.Error), Is.False, "Must have error");

            Assert.That(string.IsNullOrEmpty(vm["PropertyPublic"]), Is.True, "Must have not error on PropertyPublic");
            Assert.That(string.IsNullOrEmpty(vm["Child"]), Is.True, "Must have not error on Child");
            Assert.That(string.IsNullOrEmpty(vm.Child["PropertyPublic"]), Is.False, "Must have error on Child.PropertyPublic");
            Assert.That(string.IsNullOrEmpty(vm.Child["PropertyWithProtectedGet"]), Is.False, "Must have error on Child.PropertyWithProtectedGet");

            vm.Child.PropertyPublic = "ahahah";
            //Child.PropertyWithProtectedGet is null
            Assert.That(string.IsNullOrEmpty(vm.Error), Is.False, "Must still have error");
            vm.Child.PropertyWithProtectedGet = "bbbb";
            //Ok
            Assert.That(string.IsNullOrEmpty(vm.Error), Is.True, "Must have not error");
        }

        //Used by reflection
        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // ReSharper disable UnusedMember.Local
        private class ViewModelWithValidation : NotifyPropertyChangedWithValidationBase
        {
            public ViewModelWithValidation()
            {
                AddValidationRule(new[] { nameof(PropertyPublic) }, () => string.IsNullOrWhiteSpace(PropertyPublic) ? "Null or Empty" : null);
                AddValidationRule(nameof(PropertyWithProtectedGet), new[] { () => string.IsNullOrWhiteSpace(PropertyWithProtectedGet) ? "Null or Empty" : null });
            }

            public void AddRuleWithUnknownSource()
            {
                //Unknown because only instance | public property are allowed
                AddValidationRule(nameof(Inner), () => PropertyWithProtectedGet);
            }
            public void AddRuleWithIEnumerableFuncNullRule()
            {
                AddValidationRule(nameof(PropertyPublic), (IEnumerable<Func<string>>) null);
            }
            public void AddRuleWithIEnumerableNameNullRule()
            {
                AddValidationRule((IEnumerable<string>) null, () => PropertyWithProtectedGet);
            }
            public void AddRuleWithNullRule()
            {
                AddValidationRule(nameof(PropertyPublic), (Func<string>) null);
            }

            public static string Inner { get; set; }

            private string _propertyWithProtectedGet;
            private string _propertyPublic;
            private string _propertyWithNoRule;

            public string PropertyWithNoRule
            {
                get { return _propertyWithNoRule; }
                set
                {
                    if (value != _propertyWithNoRule)
                    {
                        _propertyWithNoRule = value;
                        OnNotifyPropertyChanged(nameof(PropertyWithNoRule));
                    }
                }
            }
            public string PropertyPublic
            {
                get { return _propertyPublic; }
                set
                {
                    if (value != _propertyPublic)
                    {
                        _propertyPublic = value;
                        OnNotifyPropertyChanged(nameof(PropertyPublic));
                    }
                }
            }
            public string PropertyWithProtectedGet
            {
                protected get { return _propertyWithProtectedGet; }
                set
                {
                    if (value != _propertyWithProtectedGet)
                    {
                        _propertyWithProtectedGet = value;
                        OnNotifyPropertyChanged(nameof(PropertyWithProtectedGet));
                    }
                }
            }
        }
        private class ViewModelWithValidation2 : NotifyPropertyChangedWithValidationBase
        {
            public ViewModelWithValidation2()
            {
                AddValidationRule(nameof(PropertyPublic), () => string.IsNullOrWhiteSpace(PropertyPublic) ? "Null or Empty" : null);

            }

            public ViewModelWithValidation Child { get; set; }

            public void CreateChild()
            {
                Child = new ViewModelWithValidation();
            }

            private string _propertyPublic;

            public string PropertyPublic
            {
                get { return _propertyPublic; }
                set
                {
                    if (value != _propertyPublic)
                    {
                        _propertyPublic = value;
                        OnNotifyPropertyChanged(nameof(PropertyPublic));
                    }
                }
            }
        }
        // ReSharper restore MemberCanBePrivate.Local
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        // ReSharper restore UnusedMember.Local
    }
}