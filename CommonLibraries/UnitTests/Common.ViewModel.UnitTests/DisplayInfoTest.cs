namespace Common.ViewModel.UnitTests
{
    using NUnit.Framework;

    using Common.ViewModel.Dialog;

    [TestFixture]
    public class DisplayInfoTest
    {
        [Test]
        public void TestConstructor()
        {
            DisplayInfo vm = new DisplayInfo();

            Assert.That(vm.CancelCommandLabel, Is.EqualTo("Cancel"));
            Assert.That(vm.OkCommandLabel, Is.EqualTo("Ok"));
            Assert.That(vm.OtherCommandLabel, Is.Null);
            Assert.That(vm.Other2CommandLabel, Is.Null);
        }

        [Test]
        public void TestProperties()
        {
            DisplayInfo vm = new DisplayInfo();

            Assert.That(vm.CancelCommandLabel, Is.EqualTo("Cancel"));
            using (NotifyPropertyChangedChecker checker = new NotifyPropertyChangedChecker(vm))
            {
                vm.CancelCommandLabel = "CancelCommandLabel";
                Assert.That(vm.CancelCommandLabel, Is.EqualTo("CancelCommandLabel"));
                checker.Check(nameof(vm.CancelCommandLabel));
            }

            Assert.That(vm.OkCommandLabel, Is.EqualTo("Ok"));
            using (NotifyPropertyChangedChecker checker = new NotifyPropertyChangedChecker(vm))
            {
                vm.OkCommandLabel = "OkCommandLabel";
                Assert.That(vm.OkCommandLabel, Is.EqualTo("OkCommandLabel"));
                checker.Check(nameof(vm.OkCommandLabel));
            }

            Assert.That(vm.OtherCommandLabel, Is.Null);
            using (NotifyPropertyChangedChecker checker = new NotifyPropertyChangedChecker(vm))
            {
                vm.OtherCommandLabel = "OtherCommandLabel";
                Assert.That(vm.OtherCommandLabel, Is.EqualTo("OtherCommandLabel"));
                checker.Check(nameof(vm.OtherCommandLabel));
            }

            Assert.That(vm.Other2CommandLabel, Is.Null);
            using (NotifyPropertyChangedChecker checker = new NotifyPropertyChangedChecker(vm))
            {
                vm.Other2CommandLabel = "Other2CommandLabel";
                Assert.That(vm.Other2CommandLabel, Is.EqualTo("Other2CommandLabel"));
                checker.Check(nameof(vm.Other2CommandLabel));
            }
        }
    }
}
