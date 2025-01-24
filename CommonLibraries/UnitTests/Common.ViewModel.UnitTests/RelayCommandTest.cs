namespace Common.ViewModel.UnitTests
{
    using System;

    using NUnit.Framework;

    [TestFixture]
    public class RelayCommandTest
    {
        [Test]
        public void TestConstructorNull()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("execute"), () => new RelayCommand(null), "null execute should throw ArgumentNullException");
        }

        [Test]
        public void TestCanExecuteNull()
        {
            RelayCommand command = new RelayCommand( o => { }, null);
            Assert.That(command.CanExecute(null), Is.True);
        }
        [Test]
        public void TestCanExecute()
        {
            RelayCommand command = new RelayCommand(o => { }, (o) => o != null);
            Assert.That(command.CanExecute(null), Is.False);
            Assert.That(command.CanExecute(new object()), Is.True);
        }
        [Test]
        public void TestExecute()
        {
            object obj = null;

            RelayCommand command = new RelayCommand(o => { obj = o; } );
            Assert.That(obj, Is.Null);
            object o = new object();
            command.Execute(o);
            Assert.That(obj, Is.EqualTo(o));
        }
    }
}
