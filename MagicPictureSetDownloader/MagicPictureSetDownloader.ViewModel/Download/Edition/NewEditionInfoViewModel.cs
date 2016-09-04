namespace MagicPictureSetDownloader.ViewModel.Download.Edition
{
    using System;
    using System.Linq;
    using System.Windows.Input;

    using Common.ViewModel;
    using Common.ViewModel.Dialog;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public class NewEditionInfoViewModel : DialogViewModelBase
    {
        private string _name;
        private string _code;
        private DateTime? _releaseDate;
        private int? _cardNumber;
        private bool _hasFoil;
        private int? _position;
        private IBlock _blockSelected;
        private byte[] _icon;
        private readonly IMagicDatabaseReadAndWriteReference _magicDatabase;
        private readonly Func<string, byte[]> _getIcon;

        public NewEditionInfoViewModel(string gathererName, Func<string, byte[]> getIcon)
        {
            _getIcon = getIcon;
            GathererName = gathererName;
            Name = gathererName;
            HasFoil = true;
            _magicDatabase = MagicDatabaseManager.ReadAndWriteReference;
            Blocks = _magicDatabase.GetAllBlocks().Ordered().ToArray();
            ResetBlockCommand = new RelayCommand(ResetBlockExecute);
            GetIconCommand = new RelayCommand(GetIconExecute, GetIconCanExecute);

            Display.Title = "New edition";
            Display.CancelCommandLabel = "Default";
        }

        public ICommand ResetBlockCommand { get; private set; }
        public ICommand GetIconCommand { get; private set; }
        public string GathererName { get; private set; }
        public IBlock[] Blocks { get; private set; }
        public byte[] Icon
        {
            get { return _icon; }
            set
            {
                if (value != _icon)
                {
                    _icon = value;
                    OnNotifyPropertyChanged(() => Icon);
                }
            }
        }

        public int? Position
        {
            get { return _position; }
            set
            {
                if (value != _position)
                {
                    _position = value;
                    OnNotifyPropertyChanged(() => Position);
                }
            }
        }
        public IBlock BlockSelected
        {
            get { return _blockSelected; }
            set
            {
                if (value != _blockSelected)
                {
                    _blockSelected = value;
                    OnNotifyPropertyChanged(() => BlockSelected);
                }
            }
        }
        public bool HasFoil
        {
            get { return _hasFoil; }
            set
            {
                if (value != _hasFoil)
                {
                    _hasFoil = value;
                    OnNotifyPropertyChanged(() => HasFoil);
                }
            }
        }
        public int? CardNumber
        {
            get { return _cardNumber; }
            set
            {
                if (value != _cardNumber)
                {
                    _cardNumber = value;
                    OnNotifyPropertyChanged(() => CardNumber);
                }
            }
        }
        public DateTime? ReleaseDate
        {
            get { return _releaseDate; }
            set
            {
                if (value != _releaseDate)
                {
                    _releaseDate = value;
                    OnNotifyPropertyChanged(() => ReleaseDate);
                }
            }
        }
        public string Code
        {
            get { return _code; }
            set
            {
                if (value != _code)
                {
                    if (value == null)
                    {
                        _code = null;
                    }
                    else
                    {
                        _code = value.ToUpperInvariant();
                    }
                    OnNotifyPropertyChanged(() => Code);
                }
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnNotifyPropertyChanged(() => Name);
                }
            }
        }

        private void ResetBlockExecute(object o)
        {
            Position = null;
            BlockSelected = null;
        }
        private void GetIconExecute(object o)
        {
            if (_getIcon != null)
            {
                byte[] icon = _getIcon(Code);
                if (icon != null)
                {
                    Icon = icon;
                }
            }
        }
        protected override bool OkCommandCanExecute(object o)
        {
            return !string.IsNullOrWhiteSpace(Name);
        }
        private bool GetIconCanExecute(object o)
        {
            return !string.IsNullOrWhiteSpace(Code);
        }
        public void Save()
        {
            _magicDatabase.InsertNewEdition(GathererName, Name, HasFoil, Code, BlockSelected == null ? (int?)null : BlockSelected.Id, Position, CardNumber, ReleaseDate, Icon);
        }
        public void SaveDefault()
        {
            _magicDatabase.InsertNewEdition(GathererName);
        }
    }
}
