﻿namespace Common.ViewModel.Input
{
    using System.Collections.Generic;

    using Common.ViewModel.Dialog;

    public enum InputMode
    {
        Info,
        Question,
        TextNeed,
        ChooseInList,
        ChooseInListAndTextNeed,
        MoveFromListToOther,
    }

    public class InputViewModel : DialogViewModelBase
    {
        private string _text;
        private string _selected;
        private string _selected2;

        internal InputViewModel(InputMode inputMode, string title, string label) :
            this(inputMode, title, label, null, null, null)
        {
        }
        internal InputViewModel(InputMode inputMode, string title, string label, IList<string> list) :
            this(inputMode, title, label, list, null, null)
        {
        }
        internal InputViewModel(string title, string label, IList<string> list, string label2, IList<string> list2) :
            this(InputMode.MoveFromListToOther, title, label, list, label2, list2)
        {
        }

        private InputViewModel(InputMode inputMode, string title, string label, IList<string> list, string label2, IList<string> list2)
        {
            InputMode = inputMode;
            Title = title;
            Label = label;
            Label2 = label2;
            List = list;
            List2 = list2;
        }

        public InputMode InputMode { get; }
        public string Title { get; }
        public string Label { get; }
        public string Label2 { get; }
        public IList<string> List { get; }
        public IList<string> List2 { get; }

        public string Text
        {
            get { return _text; }
            set
            {
                if (value != _text)
                {
                    _text = value;
                    OnNotifyPropertyChanged(nameof(Text));
                }
            }
        }
        public string Selected
        {
            get { return _selected; }
            set
            {
                if (value != _selected)
                {
                    _selected = value;
                    OnNotifyPropertyChanged(nameof(Selected));
                }
            }
        }
        public string Selected2
        {
            get { return _selected2; }
            set
            {
                if (value != _selected2)
                {
                    _selected2 = value;
                    OnNotifyPropertyChanged(nameof(Selected2));
                }
            }
        }

        protected override bool OkCommandCanExecute(object o)
        {
            return InputMode switch
            {
                InputMode.ChooseInList => Selected != null,
                InputMode.MoveFromListToOther => Selected != null && Selected2 != null && Selected != Selected2,
                InputMode.TextNeed => !string.IsNullOrWhiteSpace(Text),
                InputMode.ChooseInListAndTextNeed => Selected != null && !string.IsNullOrWhiteSpace(Text) && Selected != Text,
                _ => true,
            };
        }
    }
}
