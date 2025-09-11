namespace Common.ViewModel.Validation
{
    using System;
    using System.ComponentModel;
    using System.Text;

    public abstract class ValidatorBase<T> : IValidator
        where T : class, INotifyPropertyChanged
    {
        private readonly IValidator _child;
        private readonly T _instance;

        protected ValidatorBase(T instance)
        {
            ArgumentNullException.ThrowIfNull(instance);
            _instance = instance;
        }
        protected ValidatorBase(ValidatorBase<T> child)
        {
            ArgumentNullException.ThrowIfNull(child);

            _child = child;
            _instance = child._instance;
        }

        public string Validate()
        {
            StringBuilder errorMessage = new StringBuilder();
            string res = PerformValidation(_instance);
            if (!string.IsNullOrWhiteSpace(res))
            {
                errorMessage.AppendLine(res);
            }

            if (_child != null)
            {
                res = _child.Validate();
                if (!string.IsNullOrWhiteSpace(res))
                {
                    errorMessage.Append(res);
                }
            }

            if (errorMessage.Length == 0)
            {
                return null;
            }

            return errorMessage.ToString();
        }

        protected abstract string PerformValidation(T instance);
    }
}