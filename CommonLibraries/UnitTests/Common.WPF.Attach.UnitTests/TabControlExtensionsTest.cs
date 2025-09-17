namespace Common.WPF.Attach.UnitTests
{
    using System.Windows;
    using System.Windows.Controls;

    using Common.WPF.Attach;

    using NUnit.Framework;

    [TestFixture]
    public class TabControlExtensionsTest
    {
        [Test]
        public void TestSetAndGetSelectOnlyVisibleTabs()
        {
            TabControl tab = new TabControl();

            TabControlExtensions.SetSelectOnlyVisibleTabs(tab, true);
            bool visible = TabControlExtensions.GetSelectOnlyVisibleTabs(tab);

            Assert.That(visible, Is.True);
        }
        [Test]
        public void TestSetAndGetSelectOnlyVisibleTabsOnWrongType()
        {
            DependencyObject d = new DependencyObject();

            TabControlExtensions.SetSelectOnlyVisibleTabs(d, true);
            bool visible = TabControlExtensions.GetSelectOnlyVisibleTabs(d);

            Assert.That(visible, Is.True);
        }

        [Test]
        public void TestAttached()
        {
            TabControl tab = new TabControl();
            tab.Items.Add(new TabItem { Header = "1", Visibility = Visibility.Collapsed });
            tab.Items.Add(new TabItem { Header = "2", Visibility = Visibility.Visible });
            tab.Items.Add(new TabItem { Header = "3", Visibility = Visibility.Visible });
            tab.SelectedIndex = 0;

            Assert.That(tab.SelectedIndex, Is.EqualTo(0));

            TabControlExtensions.SetSelectOnlyVisibleTabs(tab, true);

            Assert.That(tab.SelectedIndex, Is.EqualTo(1));
        }
        [Test]
        public void TestUnattached()
        {
            TabControl tab = new TabControl();
            tab.Items.Add(new TabItem { Header = "1", Visibility = Visibility.Collapsed });
            tab.Items.Add(new TabItem { Header = "2", Visibility = Visibility.Visible });
            tab.Items.Add(new TabItem { Header = "3", Visibility = Visibility.Visible });
            tab.SelectedIndex = 0;

            Assert.That(tab.SelectedIndex, Is.EqualTo(0));

            TabControlExtensions.SetSelectOnlyVisibleTabs(tab, false);

            Assert.That(tab.SelectedIndex, Is.EqualTo(0));
        }

        [Test]
        public void TestBehavior()
        {
            TabControl tab = new TabControl();
            tab.Items.Add(new TabItem { Header = "1", Visibility = Visibility.Collapsed });
            tab.Items.Add(new TabItem { Header = "2", Visibility = Visibility.Visible });
            tab.Items.Add(new TabItem { Header = "3", Visibility = Visibility.Visible });
            tab.SelectedIndex = 0;

            Assert.That(tab.SelectedIndex, Is.EqualTo(0));

            TabControlExtensions.SetSelectOnlyVisibleTabs(tab, true);

            Assert.That(tab.SelectedIndex, Is.EqualTo(1));

            tab.SelectedIndex = 0;
            Assert.That(tab.SelectedIndex, Is.EqualTo(1));

            TabControlExtensions.SetSelectOnlyVisibleTabs(tab, false);
            tab.SelectedIndex = 0;
            Assert.That(tab.SelectedIndex, Is.EqualTo(0));
        }
    }
}