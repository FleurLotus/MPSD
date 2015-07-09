namespace Common.UnitTests.ViewModel
{
    using Common.ViewModel.Validation;
    using Common.ViewModel.Validation.Attributes;

    using NUnit.Framework;

    [TestFixture]
    public class ValidationUsingAttributTest
    {
        [Test]
        public void TestRule()
        {
            ViewModelWithValidation vm = new ViewModelWithValidation { StringValue = "123456", IntValue = 0, ObjectValue = new object() };
            
            Assert.IsNullOrEmpty(vm.Error, "Must not have error");
            vm.ObjectValue = null;
            Assert.IsNotNullOrEmpty(vm.Error, "Must have error an other null error");
            vm.ObjectValue = "derfgtyhjuikl";
            Assert.IsNullOrEmpty(vm.Error, "Must not have error");
            vm.IntValue = -10;
            Assert.IsNotNullOrEmpty(vm.Error, "Must have error a greather than error");
            vm.IntValue = -5;
            Assert.IsNullOrEmpty(vm.Error, "Must not have error");
            vm.IntValue = 10;
            Assert.IsNotNullOrEmpty(vm.Error, "Must have error a less than error");
            vm.IntValue = 5;
            Assert.IsNotNullOrEmpty(vm.Error, "Must still have error a less than error");
            vm.IntValue = 0;
            Assert.IsNullOrEmpty(vm.Error, "Must not have error");
            vm.StringValue = null;
            Assert.IsNotNullOrEmpty(vm.Error, "Must have error a min len  error");
            vm.StringValue = "aze";
            Assert.IsNotNullOrEmpty(vm.Error, "Must still have error a min len  error");
            vm.StringValue = "azert";
            Assert.IsNullOrEmpty(vm.Error, "Must not have error");
            vm.StringValue = "azertazertazert";
            Assert.IsNotNullOrEmpty(vm.Error, "Must have error a max len  error");
            vm.StringValue = "azertazert";
            Assert.IsNullOrEmpty(vm.Error, "Must not have error");
        }

        //Used by reflection
        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // ReSharper disable UnusedMember.Local
        private class ViewModelWithValidation : NotifyPropertyChangedWithValidationBase
        {
            private int _intValue;
            private object _objectValue;
            private string _stringValue;

            [StringMaxLenValidation(10), StringMinLenValidation(5)]
            public string StringValue
            {
                get { return _stringValue; }
                set
                {
                    if (value != _stringValue)
                    {
                        _stringValue = value;
                        OnNotifyPropertyChanged(() => StringValue);
                    }
                }
            }
            [NotNullValidation]
            public object ObjectValue
            {
                get { return _objectValue; }
                set
                {
                    if (value != _objectValue)
                    {
                        _objectValue = value;
                        OnNotifyPropertyChanged(() => ObjectValue);
                    }
                }
            }
            [GreaterThanValidation(-5, true), LessThanValidation(5, false)]
            public int IntValue
            {
                get { return _intValue; }
                set
                {
                    if (value != _intValue)
                    {
                        _intValue = value;
                        OnNotifyPropertyChanged(() => IntValue);
                    }
                }
            }

        }
        // ReSharper restore MemberCanBePrivate.Local
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        // ReSharper restore UnusedMember.Local
    }
}