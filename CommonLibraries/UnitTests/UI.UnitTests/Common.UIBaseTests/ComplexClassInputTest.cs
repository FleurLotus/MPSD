namespace Common.UIBaseTests
{
    using System.Threading.Tasks;
    using System.Windows.Controls;

    using NUnit.Framework;

    using XamlTest;

    [TestFixture]
    public class ComplexClassInputTest : TestBase
    {
        [Test]
        public async Task SampleSetAndGetDataContext()
        {
            await InitializeWithDesign();
            IWindow window = await App.CreateWindow<SampleWindow>();
            SampleWindowViewModel vm = new SampleWindowViewModel();

            IVisualElement<Label> label = await window.GetElement(ElementQuery.OfType<Label>().AtIndex(0));
            string text = await label.GetProperty<string>("Content");
            Assert.That(text, Is.EqualTo(vm.SampleText));

            //No change
            vm.SampleText = "New Sample Text";
            label = await window.GetElement(ElementQuery.OfType<Label>().AtIndex(0));
            text = await label.GetProperty<string>("Content");
            Assert.That(text, Is.Not.EqualTo(vm.SampleText));

            //Inject the new viewmodel into the DataContext as string with type info
            await window.SetProperty("DataContext", vm.ToString(), vm.GetType().AssemblyQualifiedName);
            label = await window.GetElement(ElementQuery.OfType<Label>().AtIndex(0));
            text = await label.GetProperty<string>("Content");
            Assert.That(text, Is.EqualTo(vm.SampleText));

            // Needed for the wait back from DataContext to work correctly if ToString is not overridden
            await RegisterSerializer<SampleWindowViewModelSerializer>();
            object o = new SampleWindowViewModelSerializer().Deserialize(typeof(SampleWindowViewModel), (await window.GetDataContext()).ToString(), null);
            SampleWindowViewModel rvm = o as SampleWindowViewModel;
            Assert.That(rvm, Is.Not.Null);
            Assert.That(rvm.SampleText, Is.EqualTo(vm.SampleText));
        }
    }
}
