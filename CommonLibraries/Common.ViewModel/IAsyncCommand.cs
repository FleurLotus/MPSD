namespace Common.ViewModel
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    //From https://johnthiriet.com/mvvm-going-async-with-async-command/# and https://github.com/johnthiriet/AsyncVoid/blob/master/AsyncVoid
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();
        bool CanExecute();
    }
    public interface IAsyncCommand<T> : ICommand
    {
        Task ExecuteAsync(T parameter);
        bool CanExecute(T parameter);
    }
}
