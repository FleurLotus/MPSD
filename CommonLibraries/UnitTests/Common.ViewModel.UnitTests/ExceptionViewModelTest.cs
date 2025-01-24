namespace Common.ViewModel.UnitTests
{
    using System;

    using NUnit.Framework;

    using Common.ViewModel.Exception;

    [TestFixture]
    public class ExceptionViewModelTest
    {
        [Test]
        public void TestConstructor()
        {
            ExceptionViewModel vm = new ExceptionViewModel(new Exception("Error"));

            Assert.That(vm.ExceptionText, Is.EqualTo("Error\r\n"));

            vm = new ExceptionViewModel(new Exception("Error", new Exception("Error2", new Exception("Error3"))));

            Assert.That(vm.ExceptionText, Is.EqualTo("Error\r\n\tError2\r\n\t\tError3\r\n"));
        }
    }
}
