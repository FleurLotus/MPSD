namespace Common.ViewModel.UnitTests
{
    using Common.ViewModel.Web;

    using NUnit.Framework;

    [TestFixture]
    public class CredentialInputViewModelTest
    {
        [Test]
        public void TestConstructor()
        {
            CredentialInputViewModel vm = new CredentialInputViewModel();
            Assert.That(vm.Display.Title, Is.EqualTo("Proxy Credential"));
        }

        [Test]
        public void TestProperties()
        {
            CredentialInputViewModel vm = new CredentialInputViewModel();

            Assert.That(vm.Login, Is.Null);
            using (NotifyPropertyChangedChecker checker = new NotifyPropertyChangedChecker(vm))
            {
                vm.Login = "Login";
                Assert.That(vm.Login, Is.EqualTo("Login"));
                checker.Check(nameof(vm.Login));
            }

            Assert.That(vm.Password, Is.Null);
            using (NotifyPropertyChangedChecker checker = new NotifyPropertyChangedChecker(vm))
            {
                vm.Password = "Password";
                Assert.That(vm.Password, Is.EqualTo("Password"));
                checker.Check(nameof(vm.Password));
            }
        }
    }
}