namespace Common.ViewModel.Validation.Attributes
{
    using System;

    public class LessThanValidationAttribute : ValidationAttribute
    {
        private readonly double _comparaisonValue;
        private readonly bool _allowEquals;

        public LessThanValidationAttribute(double comparaisonValue, bool allowEquals)
            : this(comparaisonValue, allowEquals, "Value must be less (or equals) than " + comparaisonValue)
        {
        }

        public LessThanValidationAttribute(double comparaisonValue, bool allowEquals, string errorMessage)
            : base(errorMessage)
        {
            _comparaisonValue = comparaisonValue;
            _allowEquals = allowEquals;
        }

        protected override bool IsValide(object instance)
        {
            if (instance == null)
            {
                return false;
            }

            try
            {
                double d = Convert.ToDouble(instance);
                int ret = _comparaisonValue.CompareTo(d);
                return ret > 0 || (ret == 0 && _allowEquals);
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }
    }
}
