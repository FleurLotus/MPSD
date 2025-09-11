namespace Common.WPF.UnitTests
{
    using System.Windows.Data;

    using Common.WPF.Binding;

    using NUnit.Framework;

    [TestFixture]
    public class TwoWayBindingTest
    {
        [Test]
        public void TestConstructor()
        {
            TwoWayBinding binding = new TwoWayBinding();

            Assert.That(binding.Mode, Is.EqualTo(BindingMode.TwoWay));
            Assert.That(binding.ValidatesOnExceptions, Is.True);
            Assert.That(binding.ValidatesOnDataErrors, Is.True);
            Assert.That(binding.NotifyOnSourceUpdated, Is.True);
            Assert.That(binding.NotifyOnTargetUpdated, Is.True);
            Assert.That(binding.NotifyOnValidationError, Is.True);
            Assert.That(binding.UpdateSourceTrigger, Is.EqualTo(UpdateSourceTrigger.PropertyChanged));
        }
        [Test]
        public void TestConstructor2()
        {
            TwoWayBinding binding = new TwoWayBinding("Property");

            Assert.That(binding.Path, Is.Not.Null);
            Assert.That(binding.Path.Path, Is.EqualTo("Property"));
            Assert.That(binding.Mode, Is.EqualTo(BindingMode.TwoWay));
            Assert.That(binding.ValidatesOnExceptions, Is.True);
            Assert.That(binding.ValidatesOnDataErrors, Is.True);
            Assert.That(binding.NotifyOnSourceUpdated, Is.True);
            Assert.That(binding.NotifyOnTargetUpdated, Is.True);
            Assert.That(binding.NotifyOnValidationError, Is.True);
            Assert.That(binding.UpdateSourceTrigger, Is.EqualTo(UpdateSourceTrigger.PropertyChanged));
        }
    }
}