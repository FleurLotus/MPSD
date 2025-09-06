namespace Common.ViewModel.UnitTests
{
    using System;

    using NUnit.Framework;

    using Common.Notify;
    using Common.ViewModel.Input;
    using Common.ViewModel.Dialog;

    [TestFixture]
    public class DialogViewModelBaseTest
    {
        [Test]
        public void TestConstructor()
        {
            DialogViewModelBaseTesting vm = new DialogViewModelBaseTesting();

            Assert.That(vm.OkCommand, Is.Not.Null);
            Assert.That(vm.OkCommand.CanExecute(null), Is.True);
            Assert.That(vm.CancelCommand, Is.Not.Null);
            Assert.That(vm.CancelCommand.CanExecute(null), Is.True);
            Assert.That(vm.OtherCommand, Is.Not.Null);
            Assert.That(vm.OtherCommand.CanExecute(null), Is.True);
            Assert.That(vm.Other2Command, Is.Not.Null);
            Assert.That(vm.Other2Command.CanExecute(null), Is.True);
            Assert.That(vm.Display, Is.Not.Null);

            Assert.That(vm.Result, Is.Null);
        }

        [Test]
        public void TestCancelCommand()
        {
            DialogViewModelBaseTesting vm = new DialogViewModelBaseTesting();

            object sender = null;
            EventArgs ea = null; 

            vm.Closing += (s, e) =>
            {
                sender = s;
                ea = e;
            }; 
            Assert.That(vm.Result, Is.Null);
            vm.CancelCommand.Execute(null);
            Assert.That(vm.Result, Is.False);
            Assert.That(sender, Is.EqualTo(vm));
            Assert.That(ea, Is.Not.Null);
        }
        [Test]
        public void TestOkCommand()
        {
            DialogViewModelBaseTesting vm = new DialogViewModelBaseTesting();

            object sender = null;
            EventArgs ea = null;

            vm.Closing += (s, e) =>
            {
                sender = s;
                ea = e;
            };
            Assert.That(vm.Result, Is.Null);
            vm.OkCommand.Execute(null);
            Assert.That(vm.Result, Is.True);
            Assert.That(sender, Is.EqualTo(vm));
            Assert.That(ea, Is.Not.Null);
        }
        [Test]
        public void TestOtherCommand()
        {
            DialogViewModelBaseTesting vm = new DialogViewModelBaseTesting();

            object sender = null;
            EventArgs ea = null;

            vm.Closing += (s, e) =>
            {
                sender = s;
                ea = e;
            };
            Assert.That(vm.Result, Is.Null);
            vm.OtherCommand.Execute(null);
            Assert.That(vm.Result, Is.Null);
            Assert.That(sender, Is.Null);
            Assert.That(ea, Is.Null);
        }
        [Test]
        public void TestOther2Command()
        {
            DialogViewModelBaseTesting vm = new DialogViewModelBaseTesting();

            object sender = null;
            EventArgs ea = null;

            vm.Closing += (s, e) =>
            {
                sender = s;
                ea = e;
            };
            Assert.That(vm.Result, Is.Null);
            vm.Other2Command.Execute(null);
            Assert.That(vm.Result, Is.Null);
            Assert.That(sender, Is.Null);
            Assert.That(ea, Is.Null);
        }
        [Test]
        public void TestClose()
        {
            DialogViewModelBaseTesting vm = new DialogViewModelBaseTesting();

            object sender = null;
            EventArgs ea = null;

            vm.Closing += (s, e) =>
            {
                sender = s;
                ea = e;
            };
            vm.ForceClosing();
            Assert.That(sender, Is.EqualTo(vm));
            Assert.That(ea, Is.Not.Null);
        }
        [Test]
        public void TestDialogWanted()
        {
            DialogViewModelBaseTesting vm = new DialogViewModelBaseTesting();

            object sender = null;
            EventArgs<DialogViewModelBase> ea = null;

            vm.DialogWanted += (s, e) =>
            {
                sender = s;
                ea = e;
            };

            DialogViewModelBaseTesting vm2 = new DialogViewModelBaseTesting();

            vm.ForceDialogWanted(vm2);
            Assert.That(sender, Is.EqualTo(vm));
            Assert.That(ea, Is.Not.Null);
            Assert.That(ea.Data, Is.EqualTo(vm2));
        }
        [Test]
        public void TestInputRequested()
        {
            DialogViewModelBaseTesting vm = new DialogViewModelBaseTesting();

            object sender = null;
            EventArgs<InputViewModel> ea = null;

            vm.InputRequested += (s, e) =>
            {
                sender = s;
                ea = e;
            };

            InputViewModel vm2 = InputViewModelFactory.Instance.CreateTextViewModel("title", "label");

            vm.ForceInputRequested(vm2);
            Assert.That(sender, Is.EqualTo(vm));
            Assert.That(ea, Is.Not.Null);
            Assert.That(ea.Data, Is.EqualTo(vm2));
        }
    }
}
