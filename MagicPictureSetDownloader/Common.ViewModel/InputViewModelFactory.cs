
namespace Common.ViewModel
{
    using System;
    using System.Collections.Generic;

    public class InputViewModelFactory
    {
        private static readonly Lazy<InputViewModelFactory> _lazy = new Lazy<InputViewModelFactory>(() => new InputViewModelFactory());
        
        private InputViewModelFactory()
        {
        }

        public static InputViewModelFactory Instance
        {
            get { return _lazy.Value; }
        }

        public InputViewModel CreateQuestionViewModel(string title, string label)
        {
            return new InputViewModel(InputMode.Question, title, label);
        }
        public InputViewModel CreateTextViewModel(string title, string label)
        {
            return new InputViewModel(InputMode.TextNeed, title, label);
        }
        public InputViewModel CreateChooseInListViewModel(string title, string label, List<string> list)
        {
            return new InputViewModel(InputMode.ChooseInList, title, label, list);
        }
        public InputViewModel CreateChooseInListAndTextViewModel(string title, string label, List<string> list)
        {
            return new InputViewModel(InputMode.ChooseInListAndTextNeed, title, label, list);
        }
        public InputViewModel CreateMoveFromListToOtherViewModel(string title, string label, List<string> list, string label2, List<string> list2)
        {
            return new InputViewModel(title, label, list, label2, list2);
        }

    }
}
