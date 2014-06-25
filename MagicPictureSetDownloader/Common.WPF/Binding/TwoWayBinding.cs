using System.Windows.Data;

namespace Common.WPF.Binding
{
    public class TwoWayBinding : System.Windows.Data.Binding
    {

        public TwoWayBinding(string path) : base(path)
        {
            Init();
        }

        public TwoWayBinding()
        {
            Init();
        }
        private void Init()
        {
            Mode = BindingMode.TwoWay;
            ValidatesOnExceptions = true;
            ValidatesOnDataErrors = true;
            NotifyOnSourceUpdated = true;
            NotifyOnTargetUpdated = true;
            NotifyOnValidationError = true;
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        }
    }
}
