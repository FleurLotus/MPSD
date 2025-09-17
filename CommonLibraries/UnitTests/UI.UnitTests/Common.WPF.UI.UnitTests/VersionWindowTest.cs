namespace Common.WPF.UI.UnitTests
{
    using System.Threading.Tasks;
    using System.Windows.Controls;

    using Common.UIBaseTests;
    using Common.ViewModel.Version;

    using NUnit.Framework;

    using XamlTest;

    [TestFixture]
    public class VersionWindowTest : TestBase
    {
        [Test]
        public async Task TestVersionWindow()
        {
            await InitializeWithDesign();
            IWindow window = await App.CreateWindow<VersionWindow>();
            VersionViewModel vm = new VersionViewModel(typeof(App).Assembly);

            IVisualElement<Label> label = await window.GetElement(ElementQuery.OfType<Label>().AtIndex(0));
            string text = await label.GetProperty<string>("Content");
            Assert.That(text, Is.EqualTo("Name"));

            label = await window.GetElement(ElementQuery.OfType<Label>().AtIndex(1));
            text = await label.GetProperty<string>("Content");
            Assert.That(text, Is.EqualTo(vm.Name));

            label = await window.GetElement(ElementQuery.OfType<Label>().AtIndex(2));
            text = await label.GetProperty<string>("Content");
            Assert.That(text, Is.EqualTo("Number"));

            label = await window.GetElement(ElementQuery.OfType<Label>().AtIndex(3));
            text = await label.GetProperty<string>("Content");
            Assert.That(text, Is.EqualTo(vm.Version));

            label = await window.GetElement(ElementQuery.OfType<Label>().AtIndex(4));
            text = await label.GetProperty<string>("Content");
            Assert.That(text, Is.EqualTo("Copyright"));

            label = await window.GetElement(ElementQuery.OfType<Label>().AtIndex(5));
            text = await label.GetProperty<string>("Content");
            Assert.That(text, Is.EqualTo(vm.Copyright));

            label = await window.GetElement(ElementQuery.OfType<Label>().AtIndex(6));
            text = await label.GetProperty<string>("Content");
            Assert.That(text, Is.EqualTo("Info"));

            label = await window.GetElement(ElementQuery.OfType<Label>().AtIndex(7));
            text = await label.GetProperty<string>("Content");
            Assert.That(text, Is.EqualTo(vm.Description));
        }
    }
}