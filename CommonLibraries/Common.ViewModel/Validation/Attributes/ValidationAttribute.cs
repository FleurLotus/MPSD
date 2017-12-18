namespace Common.ViewModel.Validation.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public abstract class ValidationAttribute : Attribute
    {
        private readonly string _errorMessage;

        protected ValidationAttribute(string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new ArgumentNullException(nameof(errorMessage));
            }

            _errorMessage = errorMessage;
        }

        protected abstract bool IsValide(object instance);

        public string Validate(object instance)
        {
            return IsValide(instance) ? null: _errorMessage;
        }
    }
}
