namespace Common.ViewModel.Validation.Attributes
{
    using System;

    public class StringMinLenValidationAttribute : ValidationAttribute
    {
        private readonly int _minLen;

        public StringMinLenValidationAttribute(int minLen)
            : this(minLen, "Len must be minimum " + minLen)
        {
        }

        public StringMinLenValidationAttribute(int minLen, string errorMessage)
            : base(errorMessage)
        {
            if (minLen < 0)
                throw new ArgumentException("Can't be less than 0", "minLen");

            _minLen = minLen;
        }
        protected override bool IsValide(object instance)
        {
            string s = instance as string;
            if (s == null)
                return false;

            return s.Length >= _minLen;
        }
    }
}
