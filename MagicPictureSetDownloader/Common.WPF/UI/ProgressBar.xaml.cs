using System.ComponentModel;
using System.Windows;

namespace Common.WPF.UI
{
    public partial class ProgressBar
    {
        #region Members
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(ProgressBar), new PropertyMetadata(0.0, PropertyChangedCallback, CoerceValueCallback));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ProgressBar), new PropertyMetadata(default(string), PropertyChangedCallback));
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(ProgressBar), new PropertyMetadata(100.0, PropertyChangedCallback, CoerceMaximumCallback));
        public static readonly DependencyProperty ShowPerCentProperty = DependencyProperty.Register("ShowPerCent", typeof(bool), typeof(ProgressBar), new PropertyMetadata(false, PropertyChangedCallback));
        private static readonly DependencyPropertyKey _displayTextPropertyKey = DependencyProperty.RegisterReadOnly("DisplayText", typeof(string), typeof(ProgressBar), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty DisplayTextProperty = _displayTextPropertyKey.DependencyProperty;

        #endregion

        #region Constructor/Destuctor
        public ProgressBar()
        {
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
            ProgressBar pb = dependencyObject as ProgressBar;
            if (null == pb) return baseValue;
            double value = (double) baseValue;
            if (value < 0) return 0;
            if (value > pb.Maximum) return pb.Maximum;
            return baseValue;
        }
        private static object CoerceMaximumCallback(DependencyObject dependencyObject, object baseValue)
        {
            ProgressBar pb = dependencyObject as ProgressBar;
            if (null == pb) return baseValue;
            double max = (double) baseValue;
            if (max < pb.Value) return pb.Value;
            if (max < 0) return 0;
            return baseValue;
        }
        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ProgressBar pb = dependencyObject as ProgressBar;
            if (null == pb) return;
            pb.SetDisplayText();
        }
        private void SetDisplayText()
        {
            if (ShowPerCent)
            {
                double max = Maximum;
                double value = Value;
                double percent;
                if (max == value)
                    percent = 100;
                else
                    percent = value / max * 100;

                DisplayText = string.Format("{0} {1:0.00}%", Text, percent);
            }
            else
            {
                DisplayText = Text;
            }
        }
        #endregion
    }
}