namespace MagicPictureSetDownloader.ViewModel.Option
{
    using Common.ViewModel;
    using MagicPictureSetDownloader.Interface;

    public class OptionsViewModel : NotifyPropertyChangedBase
    {
        private const string ShowPictureKey = "ShowPicture";
        private bool _showPicture;
        private const string ShowVariationPictureKey = "ShowVariationPicture";
        private bool _showVariationPicture;
        private const string ShowOtherLanguagesKey = "ShowOtherLanguages";
        private bool _showOtherLanguages;
        private const string ShowStatisticsKey = "ShowStatistics";
        private bool _showStatistics;
        private const string ShowOnlyCurrentStatisticsKey = "ShowOnlyCurrentStatistics";
        private bool _showOnlyCurrentStatistics;
        private const string ShowPricesKey = "ShowPrices";
        private bool _showPrices;
        private const string AutoCheckUpgradeKey = "AutoCheckUpgrade";
        private bool _autoCheckUpgrade;

        private readonly IMagicDatabaseReadAndWriteOption _magicDatabase;

        public OptionsViewModel(IMagicDatabaseReadAndWriteOption magicDatabase)
        {
            _magicDatabase = magicDatabase;

            GetDbOptions();
        }
        public bool ShowPicture
        {
            get { return _showPicture; }
            set
            {
                if (value != _showPicture)
                {
                    _showPicture = value;
                    OnNotifyPropertyChanged(nameof(ShowPicture));
                }
            }
        }
        public bool ShowVariationPicture
        {
            get { return _showVariationPicture; }
            set
            {
                if (value != _showVariationPicture)
                {
                    _showVariationPicture = value;
                    OnNotifyPropertyChanged(nameof(ShowVariationPicture));
                }
            }
        }
        public bool ShowStatistics
        {
            get { return _showStatistics; }
            set
            {
                if (value != _showStatistics)
                {
                    _showStatistics = value;
                    OnNotifyPropertyChanged(nameof(ShowStatistics));
                }
            }
        }
        public bool ShowOnlyCurrentStatistics
        {
            get { return _showOnlyCurrentStatistics; }
            set
            {
                if (value != _showOnlyCurrentStatistics)
                {
                    _showOnlyCurrentStatistics = value;
                    OnNotifyPropertyChanged(nameof(ShowOnlyCurrentStatistics));
                }
            }
        }
        public bool ShowOtherLanguages
        {
            get { return _showOtherLanguages; }
            set
            {
                if (value != _showOtherLanguages)
                {
                    _showOtherLanguages = value;                  
                    OnNotifyPropertyChanged(nameof(ShowOtherLanguages));
                }
            }
        }
        public bool ShowPrices
        {
            get { return _showPrices; }
            set
            {
                if (value != _showPrices)
                {
                    _showPrices = value;
                    OnNotifyPropertyChanged(nameof(ShowPrices));
                }
            }
        }
        public bool AutoCheckUpgrade
        {
            get { return _autoCheckUpgrade; }
            set
            {
                if (value != _autoCheckUpgrade)
                {
                    _autoCheckUpgrade = value;
                    OnNotifyPropertyChanged(nameof(AutoCheckUpgrade));
                }
            }
        }
       
        private bool GetOptionValue(TypeOfOption typeOfOption, string optionName)
        {
            bool ret = true;
            IOption option = _magicDatabase.GetOption(typeOfOption, optionName);
            if (option != null)
            {
                ret = bool.Parse(option.Value);
            }

            return ret;
        }
        public void GetDbOptions()
        {
            ShowPicture = GetOptionValue(TypeOfOption.Display, ShowPictureKey);
            ShowVariationPicture = GetOptionValue(TypeOfOption.Display, ShowVariationPictureKey);
            ShowOtherLanguages = GetOptionValue(TypeOfOption.Display, ShowOtherLanguagesKey);
            ShowStatistics = GetOptionValue(TypeOfOption.Display, ShowStatisticsKey);
            ShowOnlyCurrentStatistics = GetOptionValue(TypeOfOption.Display, ShowOnlyCurrentStatisticsKey);
            ShowPrices = GetOptionValue(TypeOfOption.Display, ShowPricesKey);
            AutoCheckUpgrade = GetOptionValue(TypeOfOption.Upgrade, AutoCheckUpgradeKey);
        }
        public void Save()
        {
            _magicDatabase.InsertNewOption(TypeOfOption.Display, ShowPictureKey, ShowPicture.ToString());
            _magicDatabase.InsertNewOption(TypeOfOption.Display, ShowVariationPictureKey, ShowVariationPicture.ToString());
            _magicDatabase.InsertNewOption(TypeOfOption.Display, ShowStatisticsKey, ShowStatistics.ToString());
            _magicDatabase.InsertNewOption(TypeOfOption.Display, ShowOnlyCurrentStatisticsKey, ShowOnlyCurrentStatistics.ToString());
            _magicDatabase.InsertNewOption(TypeOfOption.Display, ShowOtherLanguagesKey, ShowOtherLanguages.ToString());
            _magicDatabase.InsertNewOption(TypeOfOption.Display, ShowPricesKey, ShowPrices.ToString());
            _magicDatabase.InsertNewOption(TypeOfOption.Upgrade, AutoCheckUpgradeKey, AutoCheckUpgrade.ToString());
        }
    }
}
