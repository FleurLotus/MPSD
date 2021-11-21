namespace Common.WPF.UI
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Timers;
    using System.Windows;

    public partial class ProgressBar
    {
        #region Members
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(ProgressBar), new PropertyMetadata(0.0, PropertyChangedCallback, CoerceValueCallback));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ProgressBar), new PropertyMetadata(default(string), PropertyChangedCallback));
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(ProgressBar), new PropertyMetadata(100.0, PropertyChangedCallback, CoerceMaximumCallback));
        public static readonly DependencyProperty ShowPerCentProperty = DependencyProperty.Register("ShowPerCent", typeof(bool), typeof(ProgressBar), new PropertyMetadata(false, PropertyChangedCallback));
        public static readonly DependencyProperty ShowETAProperty = DependencyProperty.Register("ShowETA", typeof(bool), typeof(ProgressBar), new PropertyMetadata(false, PropertyChangedCallback));
        private static readonly DependencyPropertyKey _displayTextPropertyKey = DependencyProperty.RegisterReadOnly("DisplayText", typeof(string), typeof(ProgressBar), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty DisplayTextProperty = _displayTextPropertyKey.DependencyProperty;

        private readonly Timer _timer;
        private readonly object _synch = new object();
        private DateTime? _startAt;
        #endregion

        #region Constructor/Destuctor
        public ProgressBar()
        {
            _timer = new Timer { Interval = 1000, AutoReset = true, Enabled = false };

            if (!Lib.IsInDesignMode())
            {
                _timer.Elapsed += TimerOnElapsed;
            }

            InitializeComponent();
        }

        #endregion

        #region Properties
        [Bindable(true), Category("Behavior")]
        public double Maximum
        {
            get { return (double) GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        [Bindable(true), Category("Behavior")]
        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        [Bindable(true), Category("Behavior")]
        public double Value
        {
            get { return (double) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        [Bindable(true), Category("Behavior")]
        public bool ShowPerCent
        {
            get { return (bool)GetValue(ShowPerCentProperty); }
            set { SetValue(ShowPerCentProperty, value); }
        }
        [Bindable(true), Category("Behavior")]
        public bool ShowETA
        {
            get { return (bool)GetValue(ShowETAProperty); }
            set { SetValue(ShowETAProperty, value); }
        }
        [Bindable(true), Category("Behavior")]
        public string DisplayText
        {
            get { return (string)GetValue(DisplayTextProperty); }
            private set { SetValue(_displayTextPropertyKey, value); }
        }
        #endregion

        #region Public methods
        #endregion

        #region Private/Protected methods
        private static object CoerceValueCallback(DependencyObject dependencyObject, object baseValue)
        {
            if (dependencyObject is not ProgressBar pb)
            {
                return baseValue;
            }

            double value = (double) baseValue;
            if (value < 0)
            {
                return 0;
            }

            if (value > pb.Maximum)
            {
                return pb.Maximum;
            }

            return baseValue;
        }
        private static object CoerceMaximumCallback(DependencyObject dependencyObject, object baseValue)
        {
            if (dependencyObject is not ProgressBar pb)
            {
                return baseValue;
            }

            double max = (double) baseValue;
            if (max < pb.Value)
            {
                return pb.Value;
            }

            if (max < 0)
            {
                return 0;
            }

            return baseValue;
        }
        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is not ProgressBar pb)
            {
                return;
            }

            pb.SetDisplayText();
        }

        private void SetDisplayText()
        {
            lock (_synch)
            {
                if (!Lib.IsInDesignMode() && !_timer.Enabled && ShowETA && Maximum != Value && Value > 0)
                {
                    _startAt = DateTime.Now;
                    _timer.Enabled = true;
                }
                if (_timer.Enabled && (!ShowETA || Maximum == Value || Value == 0))
                {
                    _startAt = null;
                    _timer.Enabled = false;
                }

                double max = Maximum;
                double value = Value;
                double percent;
                string estimatedTime = null;

                if (max == value)
                {
                    percent = 100;
                }
                else
                {
                    percent = value / max * 100;
                }

                if (_startAt.HasValue && percent > 0)
                {
                    estimatedTime = TimeSpan.FromSeconds((DateTime.Now - _startAt.Value).TotalSeconds * (100.0 - percent) / percent).ToString(@"hh\:mm\:ss");
                }

                StringBuilder sb = new StringBuilder(Text);

                if (ShowPerCent)
                {
                    sb.AppendFormat(" {0:0.00}%", percent);
                }

                if (!string.IsNullOrEmpty(estimatedTime))
                {
                    sb.Append(" ETA:" + estimatedTime);
                }

                DisplayText = sb.ToString();
            }
        }
        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Dispatcher.Invoke((Action)SetDisplayText);
        }

        #endregion
    }
}