namespace Common.WPF.Attach.UnitTests
{
    using System.Windows;
    using System.Windows.Input;

    using Common.WPF.Attach;

    using NUnit.Framework;

    [TestFixture]
    public class GridViewSortTest
    {
        [Test]
        public void TestSetAndGetAutoSort()
        {
            DependencyObject d = new DependencyObject();

            GridViewSort.SetAutoSort(d, true);
            bool autosort = GridViewSort.GetAutoSort(d);

            Assert.That(autosort, Is.True);
        }
        [Test]
        public void TestSetAndGetPropertyName()
        {
            string propertyName = "TestProperty";
            DependencyObject d = new DependencyObject();

            GridViewSort.SetPropertyName(d, propertyName);
            string s = GridViewSort.GetPropertyName(d);

            Assert.That(s, Is.EqualTo(propertyName));
        }

        [Test]
        public void TestSetAndGetCommand()
        {
            DependencyObject d = new DependencyObject();
            ICommand command = new RoutedCommand();

            GridViewSort.SetCommand(d, command);
            ICommand cmd = GridViewSort.GetCommand(d);

            Assert.That(cmd, Is.EqualTo(command));
        }
    }
}