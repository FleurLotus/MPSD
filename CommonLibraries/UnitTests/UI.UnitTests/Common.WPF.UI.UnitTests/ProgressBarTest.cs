namespace Common.WPF.UI.UnitTests
{
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Media;

    using Common.UIBaseTests;

    using NUnit.Framework;

    using XamlTest;

    [TestFixture]
    public class ProgressBarTest : TestBase
    {
        [Test]
        public async Task TestSplashScreenWindow()
        {
            await InitializeWithDesign();
            IWindow window = await App.CreateWindow<ProgressBarWindow>();

            IVisualElement<UI.ProgressBar> progressBar = await window.GetElement<UI.ProgressBar>();
            Color? b = await progressBar.GetBackgroundColor();
            Color? f = await progressBar.GetForegroundColor();

            IVisualElement<Label> label = await window.GetElement(ElementQuery.OfType<Label>().AtIndex(0));

            Color? b2 = await label.GetBackgroundColor();
            Color? f2 = await label.GetForegroundColor();
            string display = await label.GetProperty<string>("Content");
            Assert.That(display, Does.StartWith("Sample 41.67%"));
            Assert.That(display, Does.Contain("ETA"));

            Assert.That(b, Is.EqualTo(b2));
            Assert.That(f, Is.EqualTo(f2));

            label = await window.GetElement(ElementQuery.OfType<Label>().AtIndex(1));

            b2 = await label.GetBackgroundColor();
            f2 = await label.GetForegroundColor();
            //Inverted colors
            Assert.That(b, Is.EqualTo(f2));
            Assert.That(f, Is.EqualTo(b2));
            display = await label.GetProperty<string>("Content");
            Assert.That(display, Does.StartWith("Sample 41.67%"));
            Assert.That(display, Does.Contain("ETA"));

            IVisualElement<Button> button = await window.GetElement<Button>();
            await LeftClick(button);

            label = await window.GetElement(ElementQuery.OfType<Label>().AtIndex(0));
            display = await label.GetProperty<string>("Content");
            Assert.That(display, Does.StartWith("Sample 50.00%"));
            Assert.That(display, Does.Contain("ETA"));

            label = await window.GetElement(ElementQuery.OfType<Label>().AtIndex(1));
            display = await label.GetProperty<string>("Content");
            Assert.That(display, Does.StartWith("Sample 50.00%"));
            Assert.That(display, Does.Contain("ETA"));
        }
    }
}