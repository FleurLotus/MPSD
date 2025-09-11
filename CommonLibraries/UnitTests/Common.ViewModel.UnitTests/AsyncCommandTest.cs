namespace Common.ViewModel.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Common.Threading;

    using NUnit.Framework;

    [TestFixture]
    public class AsyncCommandTest
    {
        [Test]
        public void TestConstructorNull()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("execute"), () => new AsyncCommand(null), "null execute should throw ArgumentNullException");
        }

        [Test]
        public void TestCanExecuteNull()
        {
            ICommand command = new AsyncCommand(() => Task.Delay(10), null);
            Assert.That(command.CanExecute(null), Is.True);
        }
        [Test]
        public void TestCanExecute()
        {
            object o = null;

            ICommand command = new AsyncCommand(() => Task.Delay(10), () => o != null);
            Assert.That(command.CanExecute(null), Is.False);
            Assert.That(command.CanExecute(new object()), Is.False);

            o = new object();
            Assert.That(command.CanExecute(null), Is.True);
        }
        [Test]
        public void TestExecute()
        {
            object obj = null;

            ICommand command = new AsyncCommand(() =>
            {
                obj = new object();
                return Task.Delay(1);
            });

            Assert.That(obj, Is.Null);
            command.Execute(null);
            Assert.That(obj, Is.Not.Null);
        }
        [Test]
        public void TestExecuteIfCantExecute()
        {
            object obj = null;

            ICommand command = new AsyncCommand(() =>
            {
                obj = new object();
                return Task.Delay(1);
            }, () => false);

            Assert.That(obj, Is.Null);
            command.Execute(null);
            Assert.That(obj, Is.Null);
        }
        [Test]
        public async Task TestExecuteAndReExecute()
        {
            List<object> l = new List<object>();

            AsyncCommand command = new AsyncCommand(() =>
            {
                l.Add(new object());
                return Task.Delay(20);
            });

            Assert.That(l.Count, Is.EqualTo(0));
            Task t = command.ExecuteAsync();
            Assert.That(l.Count, Is.EqualTo(1));
            Task t2 = command.ExecuteAsync();
            Assert.That(l.Count, Is.EqualTo(1));
            await t;
            await t2;
            await command.ExecuteAsync();
            Assert.That(l.Count, Is.EqualTo(2));
        }
        [Test]
        public void TestExecuteWithExecption()
        {
            ErrorHangler errorHangler = new ErrorHangler();
            Exception ex = new Exception();

            ICommand command = new AsyncCommand(() => throw ex, null, errorHangler);

            Assert.That(errorHangler.GetException(), Is.Null);
            command.Execute(null);
            Assert.That(errorHangler.GetException(), Is.EqualTo(ex));
        }
    }

    [TestFixture]
    public class AsyncCommandTestType
    {
        [Test]
        public void TestConstructorNull()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("execute"), () => new AsyncCommand<object>(null), "null execute should throw ArgumentNullException");
        }

        [Test]
        public void TestCanExecuteNull()
        {
            ICommand command = new AsyncCommand<object>((o) => Task.Delay(10), null);
            Assert.That(command.CanExecute(null), Is.True);
        }
        [Test]
        public void TestCanExecute()
        {
            ICommand command = new AsyncCommand<object>(o => Task.Delay(10), o => o != null);
            Assert.That(command.CanExecute(null), Is.False);
            Assert.That(command.CanExecute(new object()), Is.True);
        }
        [Test]
        public void TestExecute()
        {
            object obj = null;

            ICommand command = new AsyncCommand<object>(o =>
            {
                obj = o;
                return Task.Delay(1);
            });

            Assert.That(obj, Is.Null);
            command.Execute(new object());
            Assert.That(obj, Is.Not.Null);
        }
        [Test]
        public void TestExecuteIfCantExecute()
        {
            object obj = null;

            ICommand command = new AsyncCommand<object>(o =>
            {
                obj = o;
                return Task.Delay(1);
            }, o => false);

            Assert.That(obj, Is.Null);
            command.Execute(new object());
            Assert.That(obj, Is.Null);
        }
        [Test]
        public async Task TestExecuteAndReExecute()
        {
            List<object> l = new List<object>();

            AsyncCommand<object> command = new AsyncCommand<object>(o =>
            {
                l.Add(new object());
                return Task.Delay(20);
            });

            Assert.That(l.Count, Is.EqualTo(0));
            Task t = command.ExecuteAsync(null);
            Assert.That(l.Count, Is.EqualTo(1));
            Task t2 = command.ExecuteAsync(null);
            Assert.That(l.Count, Is.EqualTo(1));
            await t;
            await t2;
            await command.ExecuteAsync(null);
            Assert.That(l.Count, Is.EqualTo(2));
        }

        [Test]
        public void TestExecuteWithExecption()
        {
            ErrorHangler errorHangler = new ErrorHangler();
            Exception ex = new Exception();

            ICommand command = new AsyncCommand<object>(o => throw ex, null, errorHangler);

            Assert.That(errorHangler.GetException(), Is.Null);
            command.Execute(null);
            Assert.That(errorHangler.GetException(), Is.EqualTo(ex));
        }
    }

    internal class ErrorHangler : IErrorHandler
    {
        private Exception _exception;

        public Exception GetException()
        {
            return _exception;
        }
        public void Reset()
        {
            _exception = null;
        }
        public void HandleError(Exception ex)
        {
            _exception = ex;
        }
    }
}