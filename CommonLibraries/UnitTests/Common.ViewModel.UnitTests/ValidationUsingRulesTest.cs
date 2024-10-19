namespace Common.ViewModel.UnitTests
{
    using System;

    using Common.ViewModel.Validation;

    using NUnit.Framework;

    [TestFixture]
    public class ValidationUsingRulesTest
    {
        [Test]
        public void TestUnknownProperty()
        {
            Assert.Throws<ArgumentException>(() => (new ViewModelWithValidation()).AddRuleWithUnknownSource(), "Unknown source must throw ArgumentException");
        }
        [Test]
        public void TestNullRule()
        {
            Assert.Throws<ArgumentNullException>(() => (new ViewModelWithValidation()).AddRuleWithNullRule(), "Null rule must throw ArgumentNullException");
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
                AddValidationRule(nameof(PropertyPublic), () => string.IsNullOrWhiteSpace(PropertyPublic) ? "Null or Empty" : null);
                AddValidationRule(nameof(PropertyWithProtectedGet), () => string.IsNullOrWhiteSpace(PropertyWithProtectedGet) ? "Null or Empty" : null);
            }

            public void AddRuleWithUnknownSource()
            {
                //Unknown because only instance | public property are allowed
                AddValidationRule(nameof(Inner), () => PropertyWithProtectedGet);
            }
            public void AddRuleWithNullRule()
            {
                AddValidationRule(nameof(PropertyPublic), (Func<string>)null);
            }

            public static string Inner { get; set; }

            private string _propertyWithProtectedGet;
            private string _propertyWithProtectedSet;
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
            public string PropertyWithProtectedSet
            {
                get { return _propertyWithProtectedSet; }
                protected set
                {
                    if (value != _propertyWithProtectedSet)
                    {
                        _propertyWithProtectedSet = value;
                        OnNotifyPropertyChanged(nameof(PropertyWithProtectedSet));
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