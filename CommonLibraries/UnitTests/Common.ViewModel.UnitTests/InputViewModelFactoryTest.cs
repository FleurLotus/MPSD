namespace Common.ViewModel.UnitTests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    using Common.ViewModel.Input;
    
    [TestFixture]
    public class InputViewModelFactoryTest
    {
        private readonly InputViewModelFactory _factory = InputViewModelFactory.Instance;

        [Test]
        public void TestCreateInfo()
        {
            InputViewModel input = _factory.CreateInfoViewModel("title", "label");

            Assert.That(input, Is.Not.Null);
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
        public void TestCreateQuestion()
        {
            InputViewModel input = _factory.CreateQuestionViewModel("title", "label");

            Assert.That(input, Is.Not.Null);
            Assert.That(input.InputMode, Is.EqualTo(InputMode.Question));
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
        public void TestCreateText()
        {
            InputViewModel input = _factory.CreateTextViewModel("title", "label");

            Assert.That(input, Is.Not.Null);
            Assert.That(input.InputMode, Is.EqualTo(InputMode.TextNeed));
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
        public void TestCreateChooseInList()
        {
            List<string> list = new List<string> { "a", "b", "c" };
            InputViewModel input = _factory.CreateChooseInListViewModel("title", "label", list); 

            Assert.That(input, Is.Not.Null);
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
        public void TestCreateChooseInListAndText()
        {
            List<string> list = new List<string> { "a", "b", "c" };
            InputViewModel input = _factory.CreateChooseInListAndTextViewModel("title", "label", list);

            Assert.That(input, Is.Not.Null);
            Assert.That(input.InputMode, Is.EqualTo(InputMode.ChooseInListAndTextNeed));
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
        public void TestCreateMoveFromListToOtherViewModel()
        {
            List<string> list = new List<string> { "a", "b", "c" };
            List<string> list2 = new List<string> { "1", "2", "3" };
            InputViewModel input = _factory.CreateMoveFromListToOtherViewModel("title", "label", list, "label2", list2);

            Assert.That(input, Is.Not.Null);
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
    }
}
