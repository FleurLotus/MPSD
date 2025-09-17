namespace Common.WPF.UI.UnitTests
{
    using System.Threading.Tasks;
    using System.Windows.Controls;

    using Common.UIBaseTests;

    using NUnit.Framework;

    using XamlTest;

    [TestFixture]
    public class SeparatorStyleSelectorTest : TestBase
    {
        [Test]
        public async Task TestSeparatorStyleSelector()
        {
            await InitializeWithDesign();
            IWindow window = await App.CreateWindow<SeparatorStyleSelectorWindow>();

            IVisualElement<MenuItem> menuItem = await window.GetElement<MenuItem>();
            await menuItem.SetIsSubmenuOpen(true);
            await Task.Delay(100); //Wait for menu to open

            IVisualElement<MenuItem> menuItem2 = await menuItem.GetElement(ElementQuery.OfType<MenuItem>().AtIndex(0));
            string text = await menuItem2.GetProperty<string>("Header");
            Assert.That(text, Is.EqualTo("Item1"));

            menuItem2 = await menuItem.GetElement(ElementQuery.OfType<MenuItem>().AtIndex(1));
            text = await menuItem2.GetProperty<string>("Header");
            Assert.That(text, Is.EqualTo("Item2"));

            menuItem2 = await menuItem.GetElement(ElementQuery.OfType<MenuItem>().AtIndex(2));
            IVisualElement<Separator> sep = await menuItem2.GetElement<Separator>();
            bool isEnabled = await sep.GetIsEnabled();
            Assert.That(isEnabled, Is.False);

            menuItem2 = await menuItem.GetElement(ElementQuery.OfType<MenuItem>().AtIndex(3));
            text = await menuItem2.GetProperty<string>("Header");
            Assert.That(text, Is.EqualTo("Item3"));
        }
    }
}