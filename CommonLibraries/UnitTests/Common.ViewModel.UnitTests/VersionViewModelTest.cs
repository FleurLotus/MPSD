using System.Reflection;

//Force the value for the unit tests
[assembly: AssemblyDescription("Common ViewModel Unit Tests")]

namespace Common.ViewModel.UnitTests
{
    using Common.ViewModel.Version;
    using NUnit.Framework;
    using System.Reflection;

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

        [Test]
        public void TestConstructorWithParameter()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            VersionViewModel vm = new VersionViewModel(assembly);

            Assert.That(vm.Copyright, Is.EqualTo((assembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute)) as AssemblyCopyrightAttribute)?.Copyright));
            Assert.That(vm.Description, Is.EqualTo((assembly.GetCustomAttribute(typeof(AssemblyDescriptionAttribute)) as AssemblyDescriptionAttribute)?.Description));
            Assert.That(vm.Name, Is.EqualTo(assembly.GetName().Name));
            Assert.That(vm.Version, Is.EqualTo(assembly.GetName().Version.ToString()));
        }

    }
}
