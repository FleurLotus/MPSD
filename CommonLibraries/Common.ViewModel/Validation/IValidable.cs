namespace Common.ViewModel.Validation
{
    using System.ComponentModel;

    public interface IValidable : IDataErrorInfo
    {
        string Validate();
    }
}