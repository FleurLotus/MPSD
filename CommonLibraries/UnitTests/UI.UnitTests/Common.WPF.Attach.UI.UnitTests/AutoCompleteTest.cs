namespace Common.WPF.Attach.UI.UnitTests
{
    using System.Threading.Tasks;
    using System.Windows.Controls;

    using Common.UIBaseTests;

    using NUnit.Framework;

    using XamlTest;

    [TestFixture]
    public class AutoCompleteTest : TestBase
    {
        [Test]
        public async Task TestAutoCompleteCaseInsensitive()
        {
            await InitializeWithDesign();
            IWindow window = await App.CreateWindow<AutoCompleteWindow>();
            IVisualElement<ComboBox> comboBox = await window.GetElement(ElementQuery.OfType<ComboBox>().AtIndex(0));
            IVisualElement<TextBox> textBox = await comboBox.GetElement<TextBox>("PART_EditableTextBox");
            await textBox.SendKeyboardInput($"Ap");

            string text = await textBox.GetProperty<string>("Text");
            Assert.That(text, Is.EqualTo("Apple"));

            await comboBox.SetProperty("IsDropDownOpen", true);

            await ExpectedText("Apple", comboBox, 0);
            await ExpectedText("aPPle", comboBox, 1);
            await ExpectedText("Apricot", comboBox, 2);
        }
        [Test]
        public async Task TestAutoCompleteCaseSensitive()
        {
            await InitializeWithDesign();
            IWindow window = await App.CreateWindow<AutoCompleteWindow>();
            IVisualElement<ComboBox> comboBox = await window.GetElement(ElementQuery.OfType<ComboBox>().AtIndex(1));
            IVisualElement<TextBox> textBox = await comboBox.GetElement<TextBox>("PART_EditableTextBox");
            await textBox.SendKeyboardInput($"Ap");

            string text = await textBox.GetProperty<string>("Text");
            Assert.That(text, Is.EqualTo("Apple"));

            await comboBox.SetProperty("IsDropDownOpen", true);

            await ExpectedText("Apple", comboBox, 0);
            await ExpectedText("Apricot", comboBox, 1);
        }
        private async Task ExpectedText(string expected, IVisualElement<ComboBox> comboBox, int index)
        {
            IVisualElement<ComboBoxItem> comboBoxItem = await comboBox.GetElement(ElementQuery.OfType<ComboBoxItem>().AtIndex(index));
            string text = await comboBoxItem.GetProperty<string>("Content");
            Assert.That(text, Is.EqualTo(expected));
        }
    }
}