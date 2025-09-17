namespace Common.WPF.Attach.UnitTests
{
    using System.Windows;
    using System.Windows.Controls;

    using Common.WPF.Attach;

    using NUnit.Framework;

    [TestFixture]
    public class FocusBehaviorTest
    {
        [Test]
        public void TestSetAndGetFocusFirst()
        {
            Control d = new Control();

            FocusBehavior.SetFocusFirst(d, true);
            bool enabled = FocusBehavior.GetFocusFirst(d);

            Assert.That(enabled, Is.True);
        }

        [Test]
        public void TestSetAndGetIsFocused()
        {
            DependencyObject d = new DependencyObject();

            FocusBehavior.SetIsFocused(d, true);
            bool b = FocusBehavior.GetIsFocused(d);

            Assert.That(b, Is.True);
        }
    }
}