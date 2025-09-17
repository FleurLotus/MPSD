namespace Common.WPF.Attach.UI.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Controls;

    using Common.UIBaseTests;

    using NUnit.Framework;

    using XamlTest;

    [TestFixture]
    public class GridViewSortTest : TestBase
    {
        [Test]
        public async Task TestSorting()
        {
            await InitializeWithDesign();
            IWindow window = await App.CreateWindow<GridViewSortWindow>();

            List<GridViewSortItem> list = new GridViewSortWindowViewModel().Elements.ToList();

            await Compare(window, list, 0);

            list.Sort((x, y) => x.Value.CompareTo(y.Value));
            await ClickHeader(window, 0);
            await Compare(window, list, 0);

            //Reverse sort
            list.Sort((x, y) => y.Value.CompareTo(x.Value));
            await ClickHeader(window, 0);
            await Compare(window, list, 0);
        }
        [Test]
        public async Task TestWithCommand()
        {
            await InitializeWithDesign();
            IWindow window = await App.CreateWindow<GridViewSortWindow>();

            List<GridViewSortItem> list = new GridViewSortWindowViewModel().Elements.ToList();

            await Compare(window, list, 1);

            await ClickHeader(window, 1);
            list.Add(new GridViewSortItem("EdwardValue", 200));
            await Compare(window, list, 1);
        }

        private async Task ClickHeader(IWindow window, int index)
        {
            // it seems that :
            // 0 is hidden header
            // 1 is Value
            // 2 is Name 
            // 3 is the filler

            IVisualElement<ListView> listView = await window.GetElement(ElementQuery.OfType<ListView>().AtIndex(index));
            IVisualElement<GridViewColumnHeader> gridViewColumHeader = await listView.GetElement(ElementQuery.OfType<GridViewColumnHeader>().AtIndex(1));
            IVisualElement<TextBlock> textBlock = await gridViewColumHeader.GetElement(ElementQuery.OfType<TextBlock>().AtIndex(0));
            string text = await textBlock.GetProperty<string>("Text");

            Assert.That(text, Is.EqualTo("Value"));

            await LeftClick(textBlock);
        }

        private async Task Compare(IWindow window, List<GridViewSortItem> list, int index)
        {
            IVisualElement<ListView> listView = await window.GetElement(ElementQuery.OfType<ListView>().AtIndex(index));
            IVisualElement<TextBlock> textBlock;
            string text;

            for (int i = 0; i < list.Count; i++)
            {
                IVisualElement<ListViewItem> listViewItem = await listView.GetElement(ElementQuery.OfType<ListViewItem>().AtIndex(i));

                textBlock = await listViewItem.GetElement(ElementQuery.OfType<TextBlock>().AtIndex(0));
                text = await textBlock.GetProperty<string>("Text");
                Assert.That(text, Is.EqualTo(list[i].Name));

                textBlock = await listViewItem.GetElement(ElementQuery.OfType<TextBlock>().AtIndex(1));
                text = await textBlock.GetProperty<string>("Text");
                Assert.That(text, Is.EqualTo(list[i].Value.ToString()));
            }
        }
    }
}