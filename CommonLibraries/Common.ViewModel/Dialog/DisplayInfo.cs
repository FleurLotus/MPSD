﻿namespace Common.ViewModel.Dialog
{
    public class DisplayInfo : NotifyPropertyChangedBase
    {
        private string _title;
        private string _okCommandLabel;
        private string _cancelCommandLabel;
        private string _otherCommandLabel;
        private string _other2CommandLabel;

        public DisplayInfo()
        {
            CancelCommandLabel = "Cancel";
            OkCommandLabel = "Ok";
            OtherCommandLabel = null;
            Other2CommandLabel = null;
        }
        
        public string CancelCommandLabel
        {
            get { return _cancelCommandLabel; }
            set
            {
                if (value != _cancelCommandLabel)
                {
                    _cancelCommandLabel = value;
                    OnNotifyPropertyChanged(nameof(CancelCommandLabel));
                }
            }
        }
        public string OkCommandLabel
        {
            get { return _okCommandLabel; }
            set
            {
                if (value != _okCommandLabel)
                {
                    _okCommandLabel = value;
                    OnNotifyPropertyChanged(nameof(OkCommandLabel));
                }
            }
        }
        public string Other2CommandLabel
        {
            get { return _other2CommandLabel; }
            set
            {
                if (value != _other2CommandLabel)
                {
                    _other2CommandLabel = value;
                    OnNotifyPropertyChanged(nameof(Other2CommandLabel));
                }
            }
        }
        public string OtherCommandLabel
        {
            get { return _otherCommandLabel; }
            set
            {
                if (value != _otherCommandLabel)
                {
                    _otherCommandLabel = value;
                    OnNotifyPropertyChanged(nameof(OtherCommandLabel));
                }
            }
        }
        public string Title
        {
            get { return _title; }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    OnNotifyPropertyChanged(nameof(Title));
                }
            }
        }
    }
}
