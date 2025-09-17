namespace Common.WPF.Attach.UnitTests
{
    using System.Windows;
    using System.Windows.Controls;

    using Common.WPF.Attach;

    using NUnit.Framework;

    [TestFixture]
    public class PasswordHelperTest
    {
        [Test]
        public void TestSetAndGetAttach()
        {
            PasswordBox combo = new PasswordBox();

            PasswordHelper.SetAttach(combo, true);
            bool attach = PasswordHelper.GetAttach(combo);

            Assert.That(attach, Is.True);
        }
        [Test]
        public void TestSetAndGetAttachOnWrongType()
        {
            DependencyObject d = new DependencyObject();

            PasswordHelper.SetAttach(d, true);
            bool attach = PasswordHelper.GetAttach(d);

            Assert.That(attach, Is.True);
        }
        [Test]
        public void TestSetAndGetPassword()
        {
            PasswordBox combo = new PasswordBox();

            PasswordHelper.SetPassword(combo, "aaaa");
            string password = PasswordHelper.GetPassword(combo);

            Assert.That(password, Is.EqualTo("aaaa"));
        }
        [Test]
        public void TestSetAndGetPasswordOnWrongType()
        {
            DependencyObject d = new DependencyObject();

            PasswordHelper.SetPassword(d, "aaaa");
            string password = PasswordHelper.GetPassword(d);

            Assert.That(password, Is.EqualTo("aaaa"));
        }

        [Test]
        public void TestAttached()
        {
            PasswordBox combo = new PasswordBox();
            PasswordHelper.SetAttach(combo, true);
            combo.Password = "aaaa";

            string password = PasswordHelper.GetPassword(combo);
            Assert.That(password, Is.EqualTo("aaaa"));
        }
        [Test]
        public void TestNotAttached()
        {
            PasswordBox combo = new PasswordBox();
            PasswordHelper.SetAttach(combo, false);
            combo.Password = "aaaa";

            string password = PasswordHelper.GetPassword(combo);
            Assert.That(password, Is.Empty);
        }
        [Test]
        public void TestAttachedThenDetached()
        {
            PasswordBox combo = new PasswordBox();
            PasswordHelper.SetAttach(combo, true);
            combo.Password = "aaaa";

            string password = PasswordHelper.GetPassword(combo);
            Assert.That(password, Is.EqualTo("aaaa"));

            PasswordHelper.SetAttach(combo, false);
            combo.Password = "bbbb";
            password = PasswordHelper.GetPassword(combo);
            Assert.That(password, Is.EqualTo("aaaa"));
        }
    }
}