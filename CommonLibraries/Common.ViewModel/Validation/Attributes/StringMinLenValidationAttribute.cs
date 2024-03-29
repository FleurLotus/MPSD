﻿namespace Common.ViewModel.Validation.Attributes
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
            {
                throw new ArgumentException("Can't be less than 0", nameof(minLen));
            }
            _minLen = minLen;
        }
        protected override bool IsValide(object instance)
        {
            if (instance is not string s)
            {
                return false;
            }
            return s.Length >= _minLen;
        }
    }
}
