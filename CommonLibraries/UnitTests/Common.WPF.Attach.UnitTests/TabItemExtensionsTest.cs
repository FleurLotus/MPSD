namespace Common.WPF.Attach.UnitTests
{
    using System.Windows;
    using System.Windows.Controls;

    using Common.WPF.Attach;

    using NUnit.Framework;

    [TestFixture]
    public class TabItemExtensionsTest
    {
        [Test]
        public void TestSetAndGetVisibility()
        {
            TabItem tab = new TabItem();

            TabItemExtensions.SetVisibility(tab, Visibility.Collapsed);
            Visibility visible = TabItemExtensions.GetVisibility(tab);

            Assert.That(visible, Is.EqualTo(Visibility.Collapsed));
        }
        [Test]
        public void TestSetAndGetVisibilityOnWrongType()
        {
            DependencyObject d = new DependencyObject();

            TabItemExtensions.SetVisibility(d, Visibility.Collapsed);
            Visibility visible = TabItemExtensions.GetVisibility(d);

            Assert.That(visible, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void TestBehavior()
        {
            TabControl tab = new TabControl();
            tab.Items.Add(new TabItem { Header = "1", Visibility = Visibility.Visible });
            tab.Items.Add(new TabItem { Header = "2", Visibility = Visibility.Visible });
            tab.Items.Add(new TabItem { Header = "3", Visibility = Visibility.Collapsed });
            tab.SelectedIndex = 0;

            // Needed because GetSelfAndAncestors uses VisualTreeHelper.GetParent
            Window w = new Window
            {
                WindowState = WindowState.Minimized,
                Content = tab
            };
            w.Show();
            w.Hide();

            TabItemExtensions.SetVisibility(tab.Items.GetItemAt(0) as DependencyObject, Visibility.Collapsed);

            Assert.That(tab.SelectedIndex, Is.EqualTo(1));

            TabItemExtensions.SetVisibility(tab.Items.GetItemAt(2) as DependencyObject, Visibility.Collapsed);

            Assert.That(tab.SelectedIndex, Is.EqualTo(1));

            TabItemExtensions.SetVisibility(tab.Items.GetItemAt(0) as DependencyObject, Visibility.Visible);

            Assert.That(tab.SelectedIndex, Is.EqualTo(1));
        }
    }
}