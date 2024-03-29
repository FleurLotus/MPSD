﻿namespace Common.ViewModel.Validation.Attributes
{
    using System;

    public class StringMaxLenValidationAttribute : ValidationAttribute
    {
        private readonly int _maxLen;

        public StringMaxLenValidationAttribute(int maxLen)
            : this(maxLen, "Len must be maximum " + maxLen)
        {
        }

        public StringMaxLenValidationAttribute(int maxLen, string errorMessage)
            : base(errorMessage)
        {
            if (maxLen <= 0)
            {
                throw new ArgumentException("Can't be less or equal to 0", nameof(maxLen));
            }
            _maxLen = maxLen;
        }

        protected override bool IsValide(object instance)
        {
            if (instance is not string s)
            {
                return true;
            }
            return s.Length <= _maxLen;
        }
    }
}
