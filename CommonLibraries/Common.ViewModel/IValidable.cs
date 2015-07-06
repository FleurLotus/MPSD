namespace Common.ViewModel
{
    using System.ComponentModel;

    public interface IValidable : IDataErrorInfo
    {
        string Validate();
    }
}