namespace Common.WPF.Attach.UnitTests
{
    using System.Windows;

    using Common.WPF.Attach;

    using NUnit.Framework;

    [TestFixture]
    public class AutoCompleteTest
    {
        [Test]
        public void TestSetAndGetEnabled()
        {
            DependencyObject d = new DependencyObject();

            AutoComplete.SetEnabled(d, true);
            bool enabled = AutoComplete.GetEnabled(d);

            Assert.That(enabled, Is.True);
        }

        [Test]
        public void TestSetAndGetCaseInsensitive()
        {
            DependencyObject d = new DependencyObject();

            AutoComplete.SetCaseInsensitive(d, true);
            bool b = AutoComplete.GetCaseInsensitive(d);

            Assert.That(b, Is.True);
        }
    }
}