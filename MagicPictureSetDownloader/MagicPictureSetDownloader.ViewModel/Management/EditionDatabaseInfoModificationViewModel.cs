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
        private DateTime? _releaseDate;
        private bool _hasFoil;
        private IBlock _block;
        private int? _cardNumber;

        public EditionDatabaseInfoModificationViewModel()
        {
            All.AddRange(MagicDatabase.GetAllEditionsOrdered());
            ResetBlockCommand = new RelayCommand(ResetBlockExecute, ResetBlockCanExecute);
            Blocks = MagicDatabase.GetAllBlocks().ToArray();
            Title = "Manage Edition";
        }

        public ICommand ResetBlockCommand { get; }
        public IBlock[] Blocks { get; }
        public int? CardNumber
        {
            get { return _cardNumber; }
            set
            {
                if (value != _cardNumber)
                {
                    _cardNumber = value;
                    OnNotifyPropertyChanged(nameof(CardNumber));
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
                    OnNotifyPropertyChanged(nameof(Block));
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
                    OnNotifyPropertyChanged(nameof(HasFoil));
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
                    OnNotifyPropertyChanged(nameof(ReleaseDate));
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
                    OnNotifyPropertyChanged(nameof(Code));
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
        }
        private bool ResetBlockCanExecute(object o)
        {
            return State != ChangeState.NoEdition;
        }
        protected override bool ValidateCurrent()
        {
            return base.ValidateCurrent() && !string.IsNullOrWhiteSpace(Name);
        }
        protected override void DisplayCurrent()
        {
            if (Selected == null)
            {
                Name = null;
                Code = null;
                ReleaseDate = null;
                HasFoil = false;
                Block = null;
                CardNumber = null;
            }
            else
            {
                Name = Selected.Name;
                Code = Selected.Code;
                ReleaseDate = Selected.ReleaseDate;
                HasFoil = Selected.HasFoil;
                Block = Selected.IdBlock.HasValue ? Blocks.FirstOrDefault(b => b.Id == Selected.IdBlock.Value) : null;
                CardNumber = Selected.CardNumber;
            }
        }

        protected override bool ApplyEditionToDatabase()
        {
            if (Selected == null)
            {
                return false;
            }

            MagicDatabase.UpdateEdition(Selected, Name, Name, HasFoil, Code, Block == null ? (int?)null : Block.Id, CardNumber, ReleaseDate);
            All.Clear();
            All.AddRange(MagicDatabase.GetAllEditionsOrdered());
            return true;
        }

    }
}
