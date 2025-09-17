namespace Common.WPF.Attach.UI.UnitTests
{
    using System.Threading.Tasks;
    using System.Windows.Controls;

    using Common.UIBaseTests;

    using NUnit.Framework;

    using XamlTest;

    [TestFixture]
    public class FocusBehaviorTest : TestBase
    {
        [Test]
        public async Task TestFocusBehaviorTest()
        {
            await InitializeWithDesign();
            IWindow window = await App.CreateWindow<FocusBehaviorWindow>();
            IVisualElement<TextBox> textbox = await window.GetElement(ElementQuery.OfType<TextBox>().AtIndex(0));
            bool focus = await textbox.GetIsFocused();
            Assert.That(focus, Is.True);

            //Focus the TextBox
            IVisualElement<Button> button = await window.GetElement(ElementQuery.OfType<Button>().AtIndex(0));
            await LeftClick(button);

            textbox = await window.GetElement(ElementQuery.OfType<TextBox>().AtIndex(1));
            focus = await textbox.GetIsFocused();
            Assert.That(focus, Is.True);

            //Focus the ComboBox
            button = await window.GetElement(ElementQuery.OfType<Button>().AtIndex(0));
            await LeftClick(button);

            IVisualElement<ComboBox> comboBox = await window.GetElement<ComboBox>();
            focus = await comboBox.GetIsFocused();
            Assert.That(focus, Is.True);
        }
    }
}