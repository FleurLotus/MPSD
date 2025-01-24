namespace Common.ViewModel.UnitTests
{
    using System;

    using NUnit.Framework;

    using Common.ViewModel.SplashScreen;

    [TestFixture]
    public class SplashScreenViewModelTest
    {
        [Test]
        public void TestFactory()
        {
            SplashScreenViewModel vm = SplashScreenFactory.CreateOrGetSplashScreen();
            Assert.That(vm, Is.Not.Null);

            SplashScreenViewModel vm2 = SplashScreenFactory.CreateOrGetSplashScreen();
            Assert.That(vm2, Is.EqualTo(vm));
        }
        [Test]
        public void TestConstructor()
        {
            SplashScreenViewModel vm = new SplashScreenViewModel();

            Assert.That(vm.MaxValue, Is.EqualTo(100));
        }
        [Test]
        public void TestProperties()
        {
            SplashScreenViewModel vm = new SplashScreenViewModel();

            Uri uri = new Uri("tpc://aaa");
            Assert.That(vm.SourceUri, Is.Null);
            using (NotifyPropertyChangedChecker checker = new NotifyPropertyChangedChecker(vm))
            {
                vm.SourceUri = uri;
                Assert.That(vm.SourceUri, Is.EqualTo(uri));
                checker.Check(nameof(vm.SourceUri));
            }

            Assert.That(vm.ShowProgress, Is.False);
            using (NotifyPropertyChangedChecker checker = new NotifyPropertyChangedChecker(vm))
            {
                vm.ShowProgress = true;
                Assert.That(vm.ShowProgress, Is.True);
                checker.Check(nameof(vm.ShowProgress));
            }

            Assert.That(vm.Info, Is.Null);
            using (NotifyPropertyChangedChecker checker = new NotifyPropertyChangedChecker(vm))
            {
                vm.Info = "Info";
                Assert.That(vm.Info, Is.EqualTo("Info"));
                checker.Check(nameof(vm.Info));
            }
        }
        [Test]
        public void TestCurrentValue()
        {
            SplashScreenViewModel vm = new SplashScreenViewModel();

            Assert.That(vm.CurrentValue, Is.EqualTo(0));
            using (NotifyPropertyChangedChecker checker = new NotifyPropertyChangedChecker(vm))
            {
                vm.CurrentValue = 50;
                Assert.That(vm.CurrentValue, Is.EqualTo(50));
                checker.Check(nameof(vm.CurrentValue));
            }

            using (NotifyPropertyChangedChecker checker = new NotifyPropertyChangedChecker(vm))
            {
                vm.CurrentValue = 110;
                Assert.That(vm.CurrentValue, Is.EqualTo(50));
                checker.Check(nameof(vm.CurrentValue));
            }
            using (NotifyPropertyChangedChecker checker = new NotifyPropertyChangedChecker(vm))
            {
                vm.CurrentValue = -5;
                Assert.That(vm.CurrentValue, Is.EqualTo(50));
                checker.Check(nameof(vm.CurrentValue));
            }
        }
    }
}
