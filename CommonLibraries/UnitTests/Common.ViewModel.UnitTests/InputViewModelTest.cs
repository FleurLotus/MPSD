namespace Common.ViewModel.UnitTests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    using Common.ViewModel.Input;
    
    [TestFixture]
    public class InputViewModelTest
    {
        [Test]
        public void TestConstructor()
        {
            List<string> list = new List<string> { "a", "b", "c" };
            List<string> list2 = new List<string> { "1", "2", "3" };
            InputViewModel input = new InputViewModel("title", "label", list, "label2", list2);

            Assert.That(input.InputMode, Is.EqualTo(InputMode.MoveFromListToOther));
            Assert.That(input.Title, Is.EqualTo("title"));
            Assert.That(input.Label, Is.EqualTo("label"));
            Assert.That(input.Label2, Is.EqualTo("label2"));
            Assert.That(input.List, Is.EqualTo(list));
            Assert.That(input.List2, Is.EqualTo(list2));
            Assert.That(input.Text, Is.Null);
            Assert.That(input.Selected, Is.Null);
            Assert.That(input.Selected2, Is.Null);
        }
        [Test]
        public void TestConstructor2()
        {
            InputViewModel input = new InputViewModel(InputMode.Info, "title", "label");

            Assert.That(input.InputMode, Is.EqualTo(InputMode.Info));
            Assert.That(input.Title, Is.EqualTo("title"));
            Assert.That(input.Label, Is.EqualTo("label"));
            Assert.That(input.Label2, Is.Null);
            Assert.That(input.List, Is.Null);
            Assert.That(input.List2, Is.Null);
            Assert.That(input.Text, Is.Null);
            Assert.That(input.Selected, Is.Null);
            Assert.That(input.Selected2, Is.Null);
        }
        [Test]
        public void TestConstructor3()
        {
            List<string> list = new List<string> { "a", "b", "c" };
            InputViewModel input = new InputViewModel(InputMode.ChooseInList, "title", "label", list);

            Assert.That(input.InputMode, Is.EqualTo(InputMode.ChooseInList));
            Assert.That(input.Title, Is.EqualTo("title"));
            Assert.That(input.Label, Is.EqualTo("label"));
            Assert.That(input.Label2, Is.Null);
            Assert.That(input.List, Is.EqualTo(list));
            Assert.That(input.List2, Is.Null);
            Assert.That(input.Text, Is.Null);
            Assert.That(input.Selected, Is.Null);
            Assert.That(input.Selected2, Is.Null);
        }

        [Test]
        public void TestProperties()
        {
            InputViewModel vm = new InputViewModel(InputMode.Info, "title", "label");

            Assert.That(vm.Text, Is.Null);
            using (NotifyPropertyChangedChecker checker = new NotifyPropertyChangedChecker(vm))
            {
                vm.Text = "Text";
                Assert.That(vm.Text, Is.EqualTo("Text"));
                checker.Check(nameof(vm.Text));
            }
            Assert.That(vm.Selected, Is.Null);
            using (NotifyPropertyChangedChecker checker = new NotifyPropertyChangedChecker(vm))
            {
                vm.Selected = "Selected";
                Assert.That(vm.Selected, Is.EqualTo("Selected"));
                checker.Check(nameof(vm.Selected));
            }
            Assert.That(vm.Selected2, Is.Null);
            using (NotifyPropertyChangedChecker checker = new NotifyPropertyChangedChecker(vm))
            {
                vm.Selected2 = "Selected2";
                Assert.That(vm.Selected2, Is.EqualTo("Selected2"));
                checker.Check(nameof(vm.Selected2));
            }
        }

        [Test]
        public void TestOkCommandCanExecuteInfo()
        {
            InputViewModel vm = new InputViewModel(InputMode.Info, "title", "label");

            Assert.That(vm.OkCommand.CanExecute(null), Is.True);
        }
        [Test]
        public void TestOkCommandCanExecuteQuestion()
        {
            InputViewModel vm = new InputViewModel(InputMode.Question, "title", "label");

            Assert.That(vm.OkCommand.CanExecute(null), Is.True);
        }
        [Test]
        public void TestOkCommandCanExecuteTextNeed()
        {
            InputViewModel vm = new InputViewModel(InputMode.TextNeed, "title", "label");

            Assert.That(vm.OkCommand.CanExecute(null), Is.False);
            vm.Text = "Text";
            Assert.That(vm.OkCommand.CanExecute(null), Is.True);
        }
        [Test]
        public void TestOkCommandCanExecuteChooseInList()
        {
            List<string> list = new List<string> { "a", "b", "c" };
            InputViewModel vm = new InputViewModel(InputMode.ChooseInList, "title", "label", list);

            Assert.That(vm.OkCommand.CanExecute(null), Is.False);
            vm.Selected = "a";
            Assert.That(vm.OkCommand.CanExecute(null), Is.True);
        }
        [Test]
        public void TestOkCommandCanExecuteChooseInListAndTextNeed()
        {
            List<string> list = new List<string> { "a", "b", "c" };
            InputViewModel vm = new InputViewModel(InputMode.ChooseInListAndTextNeed, "title", "label", list);

            Assert.That(vm.OkCommand.CanExecute(null), Is.False);
            vm.Selected = "a";
            Assert.That(vm.OkCommand.CanExecute(null), Is.False);
            vm.Text = "b";
            Assert.That(vm.OkCommand.CanExecute(null), Is.True);
            vm.Text = "a";
            Assert.That(vm.OkCommand.CanExecute(null), Is.False);
        }
        [Test]
        public void TestOkCommandCanExecuteMoveFromListToOther()
        {
            List<string> list = new List<string> { "a", "b", "c" };
            List<string> list2 = new List<string> { "1", "2", "3" };
            InputViewModel vm = new InputViewModel("title", "label", list, "label2", list2);

            Assert.That(vm.OkCommand.CanExecute(null), Is.False);
            vm.Selected = "a";
            Assert.That(vm.OkCommand.CanExecute(null), Is.False);
            vm.Selected2 = "b";
            Assert.That(vm.OkCommand.CanExecute(null), Is.True);
            vm.Selected2 = "a";
            Assert.That(vm.OkCommand.CanExecute(null), Is.False);
        }
    }
}
