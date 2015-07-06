namespace Common.ViewModel.Validation.Attributes
{
    using System;
    using System.Text.RegularExpressions;

    public class StringRegExValidationAttribute : ValidationAttribute
    {
        private readonly Regex _regExRule;

        public StringRegExValidationAttribute(string regExRule)
            : this(regExRule, "Must validate regular expression " + regExRule)
        {
        }

        public StringRegExValidationAttribute(string regExRule, string errorMessage)
            : base(errorMessage)
        {
            if (string.IsNullOrWhiteSpace(regExRule))
                throw new ArgumentNullException("regExRule");

            _regExRule = new Regex(regExRule);
        }
        protected override bool IsValide(object instance)
        {
            string s = instance as string;
            if (s == null)
                return false;

            return _regExRule.IsMatch(s);
        }
    }
}
