namespace Common.ViewModel
{
    using System.Collections.Generic;
    using System.Windows.Input;

    public class MenuViewModel : NotifyPropertyChangedWithLinkedPropertiesBase
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
            AddLinkedProperty(() => Children, () => HasChild);
        }

        public IList<MenuViewModel> Children
        {
            get { return _children.AsReadOnly(); }
        }
        public bool HasChild
        {
            get { return _children.Count > 0; }
        }
        public ICommand Command { get; private set; }
        public string MenuText { get; private set; }
        public bool IsSeparator { get; private set; }
        public object CommandParameter { get; private set; }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (value != _isChecked)
                {
                    _isChecked = value;
                    OnNotifyPropertyChanged(() => IsChecked);
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
                    OnNotifyPropertyChanged(() => IsCheckable);
                }
            }
        }
        
        public void AddChild(MenuViewModel child)
        {
            if (!_children.Contains(child))
            {
                _children.Add(child);
                OnNotifyPropertyChanged(() => Children);
            }
        }
        public void RemoveChild(MenuViewModel child)
        {
            if (_children.Contains(child))
            {
                _children.Remove(child);
                OnNotifyPropertyChanged(() => Children);
            }
        }
        public void RemoveAllChildren()
        {
            _children.Clear();
            OnNotifyPropertyChanged(() => Children);
        }
    }
}
