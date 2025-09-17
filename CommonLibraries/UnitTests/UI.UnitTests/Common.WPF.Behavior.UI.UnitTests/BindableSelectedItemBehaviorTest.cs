namespace Common.WPF.Behavior.UI.UnitTests
{
    using System.Threading.Tasks;
    using System.Windows.Controls;

    using Common.UIBaseTests;

    using NUnit.Framework;

    using XamlTest;

    [TestFixture]
    public class BindableSelectedItemBehavior : TestBase
    {
        [Test]
        public async Task TestSelectedItem()
        {
            await InitializeWithDesign();
            await RegisterSerializer<TreeViewModelSerializer>();
            IWindow window = await App.CreateWindow<BindableSelectedItemBehaviorWindow>();
            IVisualElement<TreeView> treeView = await window.GetElement<TreeView>();
            await LeftClick(treeView);

            IVisualElement<TreeViewItem> treeViewItem = await window.GetElement(ElementQuery.OfType<TreeViewItem>().AtIndex(0));
            IVisualElement<TextBlock> textBlock = await treeViewItem.GetElement<TextBlock>();
            await LeftClick(textBlock);

            bool selected = await treeViewItem.GetIsSelected();
            Assert.That(selected, Is.True);
            object s = await treeView.GetSelectedItem();
            Assert.That(s, Is.EqualTo("Root"));

            await treeViewItem.SetIsExpanded(true);
            IVisualElement<TreeViewItem> treeViewItem2 = await treeViewItem.GetElement(ElementQuery.OfType<TreeViewItem>().AtIndex(1));
            await treeViewItem2.SetIsExpanded(true);
            IVisualElement<TreeViewItem> treeViewItem3 = await treeViewItem2.GetElement(ElementQuery.OfType<TreeViewItem>().AtIndex(2));
            await treeViewItem3.SetIsExpanded(true);
            IVisualElement<TreeViewItem> treeViewItem4 = await treeViewItem3.GetElement(ElementQuery.OfType<TreeViewItem>().AtIndex(0));
            textBlock = await treeViewItem4.GetElement<TextBlock>();
            await LeftClick(textBlock);

            selected = await treeViewItem4.GetIsSelected();
            Assert.That(selected, Is.True);
            s = await treeView.GetSelectedItem();
            Assert.That(s, Is.EqualTo("Child 2.3.1"));
        }
    }
}