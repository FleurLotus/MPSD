﻿namespace Common.ViewModel.Menu
{
    using System.Collections.Generic;
    using System.Windows.Input;

    public class MenuViewModel : NotifyPropertyChangedBase
    {
        private bool _isCheckable;
        private bool _isChecked;
        private readonly List<MenuViewModel> _children;

        public static MenuViewModel Separator()
        {
            return new MenuViewModel(null) { IsSeparator = true };
        }

        public MenuViewModel()
            : this(null)
        {

        }

        public MenuViewModel(string text)
            : this(text, null)
        {
        }
        public MenuViewModel(string text, ICommand command)
            : this(text, command, null)
        {
        }
        public MenuViewModel(string text, ICommand command, object commandParameter)
        {
            MenuText = text;
            Command = command;
            CommandParameter = commandParameter;
            _children = new List<MenuViewModel>();
            AddLinkedProperty(nameof(Children), nameof(HasChild));
        }

        public IList<MenuViewModel> Children
        {
            get { return _children.AsReadOnly(); }
        }
        public bool HasChild
        {
            get { return _children.Count > 0; }
        }
        public ICommand Command { get; }
        public string MenuText { get; }
        public bool IsSeparator { get; private set; }
        public object CommandParameter { get; }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (value != _isChecked)
                {
                    _isChecked = value;
                    OnNotifyPropertyChanged(nameof(IsChecked));
                }
            }
        }
        public bool IsCheckable
        {
            get { return _isCheckable; }
            set
            {
                if (value != _isCheckable)
                {
                    _isCheckable = value;
                    OnNotifyPropertyChanged(nameof(IsCheckable));
                }
            }
        }
        
        public void AddChild(MenuViewModel child)
        {
            if (!_children.Contains(child))
            {
                _children.Add(child);
                OnNotifyPropertyChanged(nameof(Children));
            }
        }
        public void RemoveChild(MenuViewModel child)
        {
            if (_children.Contains(child))
            {
                _children.Remove(child);
                OnNotifyPropertyChanged(nameof(Children));
            }
        }
        public void RemoveAllChildren()
        {
            _children.Clear();
            OnNotifyPropertyChanged(nameof(Children));
        }
    }
}
