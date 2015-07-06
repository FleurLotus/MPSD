namespace Common.ViewModel.Validation.Attributes
{
    using System;

    public class GreaterThanValidationAttribute : ValidationAttribute
    {
        private readonly double _comparaisonValue;
        private readonly bool _allowEquals;

        public GreaterThanValidationAttribute(double comparaisonValue, bool allowEquals)
            : this(comparaisonValue, allowEquals, "Value must be greater (or equals) than " + comparaisonValue)
        {
        }

        public GreaterThanValidationAttribute(double comparaisonValue, bool allowEquals, string errorMessage)
            : base(errorMessage)
        {
            _comparaisonValue = comparaisonValue;
            _allowEquals = allowEquals;
        }

        protected override bool IsValide(object instance)
        {
            if (instance == null || instance.GetType().IsClass) 
                return false;

            try
            {
                double d = Convert.ToDouble(instance);
                int ret = _comparaisonValue.CompareTo(d);
                return ret < 0 || (ret == 0 && _allowEquals);
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }
    }
}
