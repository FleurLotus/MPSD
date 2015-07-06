namespace Common.ViewModel.Validation.Attributes
{
    public class NotNullValidationAttribute : ValidationAttribute
    {

        public NotNullValidationAttribute()
            : this("Must be not null")
        {
        }
        public NotNullValidationAttribute(string errorMessage)
            : base(errorMessage)
        {
        }

        protected override bool IsValide(object instance)
        {
            return instance != null;
        }
    }
}
