namespace Common.UnitTests.ViewModel
{
    using System;
    using Common.ViewModel.Validation;

    using NUnit.Framework;

    [TestFixture]
    public class GlobalValidatingTest
    {
        private const string MessageFormat = "Global validation ---> {0}\r\n";

        [Test]
        public void TestNullInstance()
        {
            Assert.Throws<ArgumentNullException>(() => new Validator((ValidatorViewModel)null), "Null instance view model must throw ArgumentNullException");
        }
        [Test]
        public void TestNullChild()
        {
            Assert.Throws<ArgumentNullException>(() => new Validator((ValidatorBase<ValidatorViewModel>)null), "Null child validator must throw ArgumentNullException");
        }
        [Test]
        public void TestWithNoRule()
        {
            ValidatorViewModel vm = new ValidatorViewModel();
            string message = vm.Validate();
            Assert.That(string.IsNullOrEmpty(message), "No rules so no error excepted");
            vm.Name = "Value";
            message = vm.Validate();
            Assert.That(string.IsNullOrEmpty(message), "No rules so no error excepted");
        }
        [Test]
        public void TestWithOkRule()
        {
            ValidatorViewModel vm = new ValidatorViewModel();
            vm.AddValidatorAlwaysOk();
            string message = vm.Validate();
            Assert.That(string.IsNullOrEmpty(message), "Rules always OK so no error excepted");
            vm.Name = "Value";
            message = vm.Validate();
            Assert.That(string.IsNullOrEmpty(message), "Rules always OK so no error excepted");
        }
        [Test]
        public void TestWithKoRule()
        {
            ValidatorViewModel vm = new ValidatorViewModel();
            vm.AddValidatorAlwaysKo();
            string message = vm.Validate();
            Assert.That(!string.IsNullOrEmpty(message), "Rules always Ko so error excepted");
            Assert.That(message, Is.EqualTo(string.Format(MessageFormat, "Error")), "Not the excepted message");
            vm.Name = "Value";
            message = vm.Validate();
            Assert.That(!string.IsNullOrEmpty(message), "Rules always Ko so error excepted");
            Assert.That(message, Is.EqualTo(string.Format(MessageFormat, "Error")), "Not the excepted message");
        }
        [Test]
        public void TestWithRealRule()
        {
            ValidatorViewModel vm = new ValidatorViewModel();
            vm.AddValidator();
            string message = vm.Validate();
            Assert.That(!string.IsNullOrEmpty(message), "Rules is not valide so error excepted");
            Assert.That(message, Is.EqualTo(string.Format(MessageFormat, "Name must not be null or white space")), "Not the excepted message");
            vm.Name = "Value";
            message = vm.Validate();
            Assert.That(string.IsNullOrEmpty(message), "Rules is valide so no error excepted");
        }
        [Test]
        public void TestWithRealRuleAndKo()
        {
            ValidatorViewModel vm = new ValidatorViewModel();
            vm.AddValidatorWithChildKo();
            string message = vm.Validate();
            Assert.That(!string.IsNullOrEmpty(message), "Rules is not valide and child always Ko so error excepted");
            Assert.That(message, Is.EqualTo(string.Format(MessageFormat, "Name must not be null or white space\r\nError")), "Not the excepted message");
            vm.Name = "Value";
            message = vm.Validate();
            Assert.That(!string.IsNullOrEmpty(message), "Rules is valide but and child always Ko so no error excepted");
            Assert.That(message, Is.EqualTo(string.Format(MessageFormat, "Error")), "Not the excepted message");
        }

        //Used by reflection
        // ReSharper disable UnusedMember.Local
        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private class ValidatorViewModel : NotifyPropertyChangedWithValidationBase
        {
            public void AddValidatorAlwaysOk()
            {
                Validator = new ValidatorAlwaysOk(this);
            }
            public void AddValidatorAlwaysKo()
            {
                Validator = new ValidatorAlwaysKo(this);
            }
            public void AddValidator()
            {
                Validator = new Validator(this);
            }
            public void AddValidatorWithChildKo()
            {
                Validator = new Validator(new ValidatorAlwaysKo(this));
            }

            private string _name;

            public string Name
            {
                get { return _name; }
                set
                {
                    if (value != _name)
                    {
                        _name = value;
                        OnNotifyPropertyChanged(nameof(Name));
                    }
                }
            }
        }
        private class Validator : ValidatorBase<ValidatorViewModel>
        {
            public Validator(ValidatorViewModel instance)
                : base(instance)
            {
            }
            public Validator(ValidatorBase<ValidatorViewModel> child)
                : base(child)
            {
            }
            protected override string PerformValidation(ValidatorViewModel instance)
            {
                return string.IsNullOrWhiteSpace(instance.Name) ? "Name must not be null or white space" : null;
            }
        }
        private class ValidatorAlwaysKo : ValidatorBase<ValidatorViewModel>
        {
            public ValidatorAlwaysKo(ValidatorViewModel instance)
                : base(instance)
            {
            }
            public ValidatorAlwaysKo(ValidatorBase<ValidatorViewModel> child)
                : base(child)
            {
            }
            protected override string PerformValidation(ValidatorViewModel instance)
            {
                return "Error";
            }
        }
        private class ValidatorAlwaysOk : ValidatorBase<ValidatorViewModel>
        {
            public ValidatorAlwaysOk(ValidatorViewModel instance)
                : base(instance)
            {
            }
            public ValidatorAlwaysOk(ValidatorBase<ValidatorViewModel> child)
                : base(child)
            {
            }
            protected override string PerformValidation(ValidatorViewModel instance)
            {
                return null;
            }
        }
        // ReSharper restore UnusedMember.Local
        // ReSharper restore MemberCanBePrivate.Local
        // ReSharper restore UnusedAutoPropertyAccessor.Local
    }
}