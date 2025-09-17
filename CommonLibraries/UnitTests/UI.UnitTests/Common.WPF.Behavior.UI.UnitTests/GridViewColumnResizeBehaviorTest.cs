namespace Common.WPF.Behavior.UI.UnitTests
{
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    using Common.UIBaseTests;

    using NUnit.Framework;

    using XamlTest;

    [TestFixture]
    public class GridViewColumnResizeBehaviorTest : TestBase
    {
        [Test]
        public async Task TestColumnResize()
        {
            await InitializeWithDesign();
            IWindow window = await App.CreateWindow<GridViewColumnResizeBehaviorWindow>();

            IVisualElement<ListView> listView = await window.GetElement(ElementQuery.OfType<ListView>().AtIndex(0));
            IVisualElement<GridViewColumnHeader> gridViewColumHeader = await listView.GetElement(ElementQuery.OfType<GridViewColumnHeader>().AtIndex(2));
            Size size = await gridViewColumHeader.GetRenderSize();
            IVisualElement<ListView> listView2 = await window.GetElement(ElementQuery.OfType<ListView>().AtIndex(1));
            IVisualElement<GridViewColumnHeader> gridViewColumHeader2 = await listView2.GetElement(ElementQuery.OfType<GridViewColumnHeader>().AtIndex(2));
            Size size2 = await gridViewColumHeader2.GetRenderSize();

            Assert.That(size2.Width, Is.EqualTo(size.Width));

            IVisualElement<Button> button = await window.GetElement(ElementQuery.OfType<Button>().AtIndex(0));
            await LeftClick(button);

            listView = await window.GetElement(ElementQuery.OfType<ListView>().AtIndex(0));
            gridViewColumHeader = await listView.GetElement(ElementQuery.OfType<GridViewColumnHeader>().AtIndex(2));
            Size size3 = await gridViewColumHeader.GetRenderSize();
            listView2 = await window.GetElement(ElementQuery.OfType<ListView>().AtIndex(1));
            gridViewColumHeader = await listView2.GetElement(ElementQuery.OfType<GridViewColumnHeader>().AtIndex(2));
            Size size4 = await gridViewColumHeader2.GetRenderSize();

            Assert.That(size4.Width, Is.EqualTo(size2.Width));
            Assert.That(size3.Width, Is.GreaterThan(size.Width));
        }
    }
}