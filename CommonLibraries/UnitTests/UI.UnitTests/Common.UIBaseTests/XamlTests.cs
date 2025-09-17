namespace Common.UIBaseTests
{
    using System.Threading.Tasks;
    using System.Windows.Controls;

    using NUnit.Framework;

    using XamlTest;

    [TestFixture]
    public class XamlTests : TestBase
    {
        [Test]
        public async Task Sample()
        {
            string xaml = @"
<ComboBox>
    <ComboBoxItem>Apple</ComboBoxItem>
    <ComboBoxItem>Banana</ComboBoxItem>
    <ComboBoxItem>Apricot</ComboBoxItem>
</ComboBox>
";
            IVisualElement<ComboBox> combo = await LoadXaml<ComboBox>(xaml);
            await combo.SetProperty("SelectedIndex", 1);
            string ret = await combo.GetProperty<string>("Text");
            Assert.That(ret, Is.EqualTo("Banana"));
        }
    }
}