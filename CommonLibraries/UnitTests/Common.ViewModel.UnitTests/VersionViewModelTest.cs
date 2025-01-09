namespace Common.ViewModel.UnitTests
{
    using System.Reflection;

    using NUnit.Framework;

    using Common.ViewModel.Version;

    [TestFixture]
    public class VersionViewModelTest
    {
        [Test]
        public void TestConstructor()
        {
            VersionViewModel vm = new VersionViewModel();

            Assembly entryAssembly = Assembly.GetEntryAssembly();
            Assert.That(vm.Copyright, Is.EqualTo((entryAssembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute)) as AssemblyCopyrightAttribute)?.Copyright));
            Assert.That(vm.Description, Is.EqualTo((entryAssembly.GetCustomAttribute(typeof(AssemblyDescriptionAttribute)) as AssemblyDescriptionAttribute)?.Description));
            Assert.That(vm.Name, Is.EqualTo(entryAssembly.GetName().Name));
            Assert.That(vm.Version, Is.EqualTo(entryAssembly.GetName().Version.ToString()));
        }
    }
}
