namespace MagicPictureSetDownloader.ViewModel.Management
{
    using System;
    using System.Linq;
    using System.Windows.Input;

    using Common.ViewModel;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Interface;

    public class EditionDatabaseInfoModificationViewModel : DatabaseInfoModificationViewModelBase<IEdition>
    {
        private string _code;
        private string _gathererName;
        private DateTime? _releaseDate;
        private bool _hasFoil;
        private IBlock _block;
        private int? _blockPosition;
        private int? _cardNumber;

        public EditionDatabaseInfoModificationViewModel()
        {
            All.AddRange(MagicDatabase.GetAllEditionsOrdered());
            ResetBlockCommand = new RelayCommand(ResetBlockExecute, ResetBlockCanExecute);
            Blocks = MagicDatabase.GetAllBlocks().ToArray();
            Title = "Manage Edition";
        }

        public ICommand ResetBlockCommand { get; private set; }
        public IBlock[] Blocks { get; private set; }
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
        public int? BlockPosition
        {
            get { return _blockPosition; }
            set
            {
                if (value != _blockPosition)
                {
                    _blockPosition = value;
                    OnNotifyPropertyChanged(() => BlockPosition);
                }
            }
        }
        public IBlock Block
        {
            get { return _block; }
            set
            {
                if (value != _block)
                {
                    _block = value;
                    OnNotifyPropertyChanged(() => Block);
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
        public string GathererName
        {
            get { return _gathererName; }
            set
            {
                if (value != _gathererName)
                {
                    _gathererName = value;
                    OnNotifyPropertyChanged(() => GathererName);
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
                    _code = value;
                    OnNotifyPropertyChanged(() => Code);
                }
            }
        }
        public override bool AllowNew
        {
            get { return false; }
        }

        private void ResetBlockExecute(object o)
        {
            Block = null;
            BlockPosition = null;
        }
        private bool ResetBlockCanExecute(object o)
        {
            return State != ChangeState.NoEdition;
        }
        protected override bool ValidateCurrent()
        {
            return base.ValidateCurrent() && !string.IsNullOrWhiteSpace(GathererName);
        }
        protected override void DisplayCurrent()
        {
            if (Selected == null)
            {
                Name = null;
                Code = null;
                GathererName = null;
                ReleaseDate = null;
                HasFoil = false;
                Block = null;
                BlockPosition = null;
                CardNumber = null;
            }
            else
            {
                Name = Selected.Name;
                Code = Selected.Code;
                GathererName = Selected.GathererName;
                ReleaseDate = Selected.ReleaseDate;
                HasFoil = Selected.HasFoil;
                Block = Selected.IdBlock.HasValue ? Blocks.FirstOrDefault(b => b.Id == Selected.IdBlock.Value) : null;
                BlockPosition = Selected.BlockPosition;
                CardNumber = Selected.CardNumber;
            }
        }

        protected override bool ApplyEditionToDatabase()
        {
            if (Selected == null)
            {
                return false;
            }

            MagicDatabase.UpdateEdition(Selected, GathererName, Name, HasFoil, Code, Block == null ? (int?)null : Block.Id, BlockPosition, CardNumber, ReleaseDate);
            All.Clear();
            All.AddRange(MagicDatabase.GetAllEditionsOrdered());
            return true;
        }

    }
}
